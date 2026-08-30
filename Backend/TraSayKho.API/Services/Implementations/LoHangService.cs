using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class LoHangService : ILoHangService
    {
        private readonly ILoHangRepository _repository;
        public LoHangService(ILoHangRepository repository) => _repository = repository;

        public async Task<List<LoHangDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<LoHangDto?> GetByIdAsync(int id)
        {
            var lh = await _repository.GetByIdAsync(id);
            return lh == null ? null : MapToDto(lh);
        }

        public async Task<List<LoHangDto>> GetBySanPhamAsync(int sanPhamId)
        {
            var list = await _repository.GetBySanPhamAsync(sanPhamId);
            return list.Select(MapToDto).ToList();
        }

        public async Task<List<LoHangDto>> GetSapHetHanAsync(int soNgayNguong)
        {
            var list = await _repository.GetSapHetHanAsync(soNgayNguong);
            return list.Select(MapToDto).ToList();
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

            // Ngay sau khi nhập lô mới, đồng bộ lại tồn kho tổng của sản phẩm
            await _repository.DongBoTonKhoSanPhamAsync(dto.SanPhamId);

            return (true, null, MapToDto(created));
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

        private static LoHangDto MapToDto(LoHang lh)
        {
            var soNgayConLai = lh.HanSuDung.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber;

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
                MucGiamGiaHienTai = lh.MucGiamGiaHienTai,
                NgayBatDauApDungGiam = lh.NgayBatDauApDungGiam,
                NgayKetThucApDungGiam = lh.NgayKetThucApDungGiam,
                TrangThai = lh.TrangThai,
                SoNgayConLaiDenHan = soNgayConLai
            };
        }
    }
}