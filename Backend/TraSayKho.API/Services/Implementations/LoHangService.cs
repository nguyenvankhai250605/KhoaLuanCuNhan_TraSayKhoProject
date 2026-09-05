using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class LoHangService : ILoHangService
    {
        private readonly ILoHangRepository _repository;
        private readonly IBacGiamGiaRepository _bacGiamGiaRepository;

        public LoHangService(ILoHangRepository repository, IBacGiamGiaRepository bacGiamGiaRepository)
        {
            _repository = repository;
            _bacGiamGiaRepository = bacGiamGiaRepository;
        }

        public async Task<List<LoHangDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            var cacBac = await _bacGiamGiaRepository.GetDangHoatDongAsync();
            return list.Select(lh => MapToDto(lh, cacBac)).ToList();
        }

        public async Task<LoHangDto?> GetByIdAsync(int id)
        {
            var lh = await _repository.GetByIdAsync(id);
            if (lh == null) return null;

            var cacBac = await _bacGiamGiaRepository.GetDangHoatDongAsync();
            return MapToDto(lh, cacBac);
        }

        public async Task<List<LoHangDto>> GetBySanPhamAsync(int sanPhamId)
        {
            var list = await _repository.GetBySanPhamAsync(sanPhamId);
            var cacBac = await _bacGiamGiaRepository.GetDangHoatDongAsync();
            return list.Select(lh => MapToDto(lh, cacBac)).ToList();
        }

        public async Task<List<LoHangDto>> GetSapHetHanAsync(int soNgayNguong)
        {
            var list = await _repository.GetSapHetHanAsync(soNgayNguong);
            var cacBac = await _bacGiamGiaRepository.GetDangHoatDongAsync();
            return list.Select(lh => MapToDto(lh, cacBac)).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage, LoHangDto? Result)> CreateAsync(LoHangCreateDto dto)
        {
            if (!await _repository.SanPhamExistsAsync(dto.SanPhamId))
                return (false, "Sản phẩm không tồn tại.", null);

            if (!await _repository.ChiNhanhExistsAsync(dto.ChiNhanhId))
                return (false, "Chi nhánh không tồn tại.", null);

            if (dto.SoLuongNhap <= 0)
                return (false, "Số lượng nhập phải lớn hơn 0.", null);

            if (dto.HanSuDung <= DateOnly.FromDateTime(DateTime.Now))
                return (false, "Hạn sử dụng phải sau ngày hiện tại.", null);

            var loHang = new LoHang
            {
                SanPhamId = dto.SanPhamId,
                ChiNhanhId = dto.ChiNhanhId,
                SoLo = dto.SoLo,
                NgayNhap = dto.NgayNhap ?? DateOnly.FromDateTime(DateTime.Now),
                HanSuDung = dto.HanSuDung,
                SoLuongNhap = dto.SoLuongNhap,
                SoLuongConLai = dto.SoLuongNhap,
                TrangThai = "ConHang"
            };

            var created = await _repository.AddAsync(loHang);
            await _repository.DongBoTonKhoSanPhamAsync(dto.SanPhamId);

            var cacBac = await _bacGiamGiaRepository.GetDangHoatDongAsync();
            return (true, null, MapToDto(created, cacBac));
        }

        public async Task<(bool Success, string? ErrorMessage)> BatXaKhoAsync(int loHangId, XaKhoDto dto)
        {
            var loHang = await _repository.GetByIdAsync(loHangId);
            if (loHang == null)
                return (false, "Không tìm thấy lô hàng.");

            if (loHang.TrangThai != "ConHang")
                return (false, "Chỉ có thể xả kho cho lô đang còn hàng.");

            if (dto.MucGiamGia <= 0 || dto.MucGiamGia > 100)
                return (false, "Mức giảm giá phải trong khoảng 0-100%.");

            if (dto.NgayKetThucApDung < dto.NgayBatDauApDung)
                return (false, "Ngày kết thúc phải sau ngày bắt đầu.");

            var success = await _repository.UpdateXaKhoAsync(
                loHangId, dto.MucGiamGia, dto.NgayBatDauApDung, dto.NgayKetThucApDung);

            return success ? (true, null) : (false, "Không thể bật xả kho.");
        }

        public async Task<(bool Success, string? ErrorMessage)> HuyXaKhoAsync(int loHangId)
        {
            var loHang = await _repository.GetByIdAsync(loHangId);
            if (loHang == null)
                return (false, "Không tìm thấy lô hàng.");

            var success = await _repository.UpdateXaKhoAsync(loHangId, null, null, null);
            return success ? (true, null) : (false, "Không thể hủy xả kho.");
        }

        // ==== HÀM QUAN TRỌNG NHẤT: quyết định mức giảm cuối cùng áp dụng cho 1 lô ====
        private static LoHangDto MapToDto(LoHang lh, List<BacGiamGiaXaKho> cacBacDangHoatDong)
        {
            var homNay = DateOnly.FromDateTime(DateTime.Now);
            var soNgayConLai = lh.HanSuDung.DayNumber - homNay.DayNumber;

            decimal? mucGiamCuoiCung;
            bool laGiamTuDong;

            // Ưu tiên 1: nếu nhân viên đã BẤM TAY xả kho (còn hiệu lực theo ngày) → dùng giá trị đó, không tự động ghi đè
            bool dangCoGiamThuCong = lh.MucGiamGiaHienTai.HasValue
                && lh.NgayBatDauApDungGiam.HasValue && lh.NgayKetThucApDungGiam.HasValue
                && homNay >= lh.NgayBatDauApDungGiam.Value && homNay <= lh.NgayKetThucApDungGiam.Value;

            if (dangCoGiamThuCong)
            {
                mucGiamCuoiCung = lh.MucGiamGiaHienTai;
                laGiamTuDong = false;
            }
            else
            {
                // Ưu tiên 2: tự động tìm bậc giảm giá phù hợp nhất theo số ngày còn lại
                // (cacBacDangHoatDong đã sắp từ ngưỡng nhỏ nhất → lớn nhất, nên bậc đầu tiên khớp là bậc gấp nhất, ưu tiên cao nhất)
                var bacPhuHop = cacBacDangHoatDong.FirstOrDefault(b => soNgayConLai <= b.SoNgayConLaiToiDa);

                mucGiamCuoiCung = bacPhuHop?.MucGiamGiaPhanTram;
                laGiamTuDong = bacPhuHop != null;
            }

            var giaSauGiam = mucGiamCuoiCung.HasValue
                ? lh.SanPham.GiaBan * (1 - mucGiamCuoiCung.Value / 100)
                : lh.SanPham.GiaBan;

            return new LoHangDto
            {
                LoHangId = lh.LoHangId,
                SanPhamId = lh.SanPhamId,
                TenSanPham = lh.SanPham.TenSanPham,
                ChiNhanhId = lh.ChiNhanhId,
                TenChiNhanh = lh.ChiNhanh.TenChiNhanh,
                SoLo = lh.SoLo,
                NgayNhap = lh.NgayNhap,
                HanSuDung = lh.HanSuDung,
                SoLuongNhap = lh.SoLuongNhap,
                SoLuongConLai = lh.SoLuongConLai,
                MucGiamGiaHienTai = mucGiamCuoiCung,
                NgayBatDauApDungGiam = lh.NgayBatDauApDungGiam,
                NgayKetThucApDungGiam = lh.NgayKetThucApDungGiam,
                TrangThai = lh.TrangThai,
                SoNgayConLaiDenHan = soNgayConLai,
                LaGiamGiaTuDong = laGiamTuDong,
                GiaSauGiam = Math.Round(giaSauGiam, 0)
            };
        }
    }
}