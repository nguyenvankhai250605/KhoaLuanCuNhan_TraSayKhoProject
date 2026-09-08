using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _repository;

        private static readonly Dictionary<string, string[]> QuyTacChuyenTrangThai = new()
        {
            ["ChoXacNhan"] = new[] { "DangXuLy", "DaHuy" },
            ["DangXuLy"] = new[] { "DangGiao", "DaHuy" },
            ["DangGiao"] = new[] { "DaGiao", "DaHuy" },
            ["DaGiao"] = new[] { "HoanThanh" },
            ["HoanThanh"] = Array.Empty<string>(),
            ["DaHuy"] = Array.Empty<string>()
        };

        public DonHangService(IDonHangRepository repository) => _repository = repository;

        public async Task<List<DonHangDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(dh => new DonHangDto
            {
                DonHangId = dh.DonHangId,
                ChiNhanhId = dh.ChiNhanhId,
                TenChiNhanh = dh.ChiNhanh?.TenChiNhanh,
                TenKhachHang = dh.KhachHang.HoTen,
                TrangThai = dh.TrangThai.TenTrangThai,
                TongTien = dh.TongTien,
                NgayDatHang = dh.NgayDatHang
            }).ToList();
        }

        public async Task<DonHangChiTietDto?> GetByIdAsync(int id)
        {
            var dh = await _repository.GetByIdWithDetailsAsync(id);
            if (dh == null) return null;
            return MapChiTietToDto(dh);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int id, CapNhatTrangThaiDto dto)
        {
            var donHangChiTiet = await _repository.GetByIdWithDetailsAsync(id);
            if (donHangChiTiet == null)
                return (false, "Không tìm thấy đơn hàng.");

            var trangThaiMoi = await _repository.GetTrangThaiByTenAsync(dto.TenTrangThaiMoi);
            if (trangThaiMoi == null)
                return (false, "Trạng thái không hợp lệ.");

            var tenTrangThaiHienTai = donHangChiTiet.TrangThai.TenTrangThai;
            if (!QuyTacChuyenTrangThai.TryGetValue(tenTrangThaiHienTai, out var cacBuocDuocPhep) ||
                !cacBuocDuocPhep.Contains(dto.TenTrangThaiMoi))
            {
                return (false, $"Không thể chuyển từ trạng thái '{tenTrangThaiHienTai}' sang '{dto.TenTrangThaiMoi}'.");
            }

            var success = await _repository.UpdateTrangThaiAsync(id, trangThaiMoi.TrangThaiId);
            return success ? (true, null) : (false, "Cập nhật thất bại.");
        }

        // ==== HÀM QUAN TRỌNG NHẤT: TẠO ĐƠN HÀNG ====
        public async Task<(bool Success, string? ErrorMessage, DonHangChiTietDto? Result)> TaoDonHangAsync(DonHangCreateDto dto)
        {
            // BƯỚC 1: Kiểm tra hợp lệ cơ bản
            if (!await _repository.KhachHangExistsAsync(dto.KhachHangId))
                return (false, "Khách hàng không tồn tại.", null);

            if (!await _repository.ChiNhanhExistsAsync(dto.ChiNhanhId))
                return (false, "Chi nhánh không tồn tại.", null);

            if (string.IsNullOrWhiteSpace(dto.DiaChiGiaoHang) || string.IsNullOrWhiteSpace(dto.SoDienThoaiNhan))
                return (false, "Vui lòng nhập đầy đủ địa chỉ và số điện thoại nhận hàng.", null);

            if (dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                return (false, "Đơn hàng phải có ít nhất 1 sản phẩm.", null);

            // BƯỚC 2: Với từng sản phẩm, tìm đúng lô theo FEFO và tính giá
            var chiTiets = new List<ChiTietDonHang>();
            var danhSachTruKho = new List<(LoHang LoHang, int SoLuongTru)>();
            decimal tienHang = 0;

            foreach (var dongDat in dto.ChiTiet)
            {
                if (dongDat.SoLuong <= 0)
                    return (false, "Số lượng sản phẩm phải lớn hơn 0.", null);

                var sanPham = await _repository.GetSanPhamAsync(dongDat.SanPhamId);
                if (sanPham == null)
                    return (false, $"Sản phẩm ID {dongDat.SanPhamId} không tồn tại.", null);

                if (sanPham.TrangThai != "DangBan")
                    return (false, $"Sản phẩm '{sanPham.TenSanPham}' hiện không còn bán.", null);

                // Lấy danh sách lô còn hàng tại đúng chi nhánh, đã sắp theo FEFO (hạn gần nhất trước)
                var cacLo = await _repository.GetLoHangConHangTheoFefoAsync(dongDat.SanPhamId, dto.ChiNhanhId);

                // Tìm lô ĐẦU TIÊN (hạn gần nhất) mà TỰ NÓ đủ số lượng đặt — không chia nhỏ qua nhiều lô
                var loPhuHop = cacLo.FirstOrDefault(lo => lo.SoLuongConLai >= dongDat.SoLuong);

                if (loPhuHop == null)
                {
                    var tongConHang = cacLo.Sum(lo => lo.SoLuongConLai);
                    return (false,
                        $"Sản phẩm '{sanPham.TenSanPham}' tại chi nhánh này không đủ hàng trong 1 lô để đáp ứng {dongDat.SoLuong} (tổng tồn kho hiện có: {tongConHang}).",
                        null);
                }

                // Tính giá bán thực tế của lô này (có thể đang được giảm giá — xả kho tay hoặc tự động)
                var giaBanThucTe = TinhGiaSauGiam(sanPham.GiaBan, loPhuHop);

                chiTiets.Add(new ChiTietDonHang
                {
                    SanPhamId = dongDat.SanPhamId,
                    LoHangId = loPhuHop.LoHangId,
                    SoLuong = dongDat.SoLuong,
                    DonGia = giaBanThucTe
                });

                danhSachTruKho.Add((loPhuHop, dongDat.SoLuong));
                tienHang += giaBanThucTe * dongDat.SoLuong;
            }

            // BƯỚC 3: Xử lý mã khuyến mãi (nếu có)
            KhuyenMai? khuyenMaiApDung = null;
            decimal tienGiamGia = 0;

            if (!string.IsNullOrWhiteSpace(dto.MaKhuyenMai))
            {
                var khuyenMai = await _repository.GetKhuyenMaiByMaCodeAsync(dto.MaKhuyenMai);

                if (khuyenMai == null)
                    return (false, "Mã khuyến mãi không tồn tại.", null);

                if (!khuyenMai.TrangThai)
                    return (false, "Mã khuyến mãi đã ngừng hoạt động.", null);

                var homNay = DateTime.Now;
                if (homNay < khuyenMai.NgayBatDau || homNay > khuyenMai.NgayKetThuc)
                    return (false, "Mã khuyến mãi không còn trong thời gian áp dụng.", null);

                if (khuyenMai.SoLuotDaSuDung >= khuyenMai.SoLuotSuDungToiDa)
                    return (false, "Mã khuyến mãi đã hết lượt sử dụng.", null);

                if (tienHang < khuyenMai.GiaTriDonHangToiThieu)
                    return (false, $"Đơn hàng cần tối thiểu {khuyenMai.GiaTriDonHangToiThieu:N0}đ để áp dụng mã này.", null);

                tienGiamGia = khuyenMai.LoaiGiam == "PhanTram"
                    ? tienHang * khuyenMai.GiaTriGiam / 100
                    : khuyenMai.GiaTriGiam;

                if (tienGiamGia > tienHang) tienGiamGia = tienHang;   // không để giảm giá vượt quá tiền hàng

                khuyenMaiApDung = khuyenMai;
            }

            var tongTien = tienHang - tienGiamGia;

            // BƯỚC 4: Lấy trạng thái khởi tạo "Chờ xác nhận"
            var trangThaiKhoiTao = await _repository.GetTrangThaiByTenAsync("ChoXacNhan");
            if (trangThaiKhoiTao == null)
                return (false, "Lỗi hệ thống: không tìm thấy trạng thái khởi tạo đơn hàng.", null);

            // BƯỚC 5: Tạo đơn hàng
            var donHang = new DonHang
            {
                KhachHangId = dto.KhachHangId,
                ChiNhanhId = dto.ChiNhanhId,
                TrangThaiId = trangThaiKhoiTao.TrangThaiId,
                KhuyenMaiId = khuyenMaiApDung?.KhuyenMaiId,
                DiaChiGiaoHang = dto.DiaChiGiaoHang,
                SoDienThoaiNhan = dto.SoDienThoaiNhan,
                PhuongThucThanhToan = dto.PhuongThucThanhToan,
                TienHang = tienHang,
                TienGiamGia = tienGiamGia,
                TongTien = tongTien,
                GhiChu = dto.GhiChu,
                NgayDatHang = DateTime.Now
            };

            var donHangDaTao = await _repository.TaoDonHangAsync(donHang, chiTiets, danhSachTruKho, khuyenMaiApDung);

            var ketQua = await _repository.GetByIdWithDetailsAsync(donHangDaTao.DonHangId);
            return (true, null, MapChiTietToDto(ketQua!));
        }

        // ==== Hàm tính giá sau giảm cho 1 lô (đồng bộ logic với LoHangService) ====
        private static decimal TinhGiaSauGiam(decimal giaGoc, LoHang loHang)
        {
            var homNay = DateOnly.FromDateTime(DateTime.Now);

            bool dangCoGiamThuCong = loHang.MucGiamGiaHienTai.HasValue
                && loHang.NgayBatDauApDungGiam.HasValue && loHang.NgayKetThucApDungGiam.HasValue
                && homNay >= loHang.NgayBatDauApDungGiam.Value && homNay <= loHang.NgayKetThucApDungGiam.Value;

            if (dangCoGiamThuCong)
                return Math.Round(giaGoc * (1 - loHang.MucGiamGiaHienTai!.Value / 100), 0);

            return giaGoc;
        }

        private static DonHangChiTietDto MapChiTietToDto(DonHang dh) => new()
        {
            DonHangId = dh.DonHangId,
            ChiNhanhId = dh.ChiNhanhId,
            TenChiNhanh = dh.ChiNhanh?.TenChiNhanh,
            TenKhachHang = dh.KhachHang.HoTen,
            TrangThai = dh.TrangThai.TenTrangThai,
            DiaChiGiaoHang = dh.DiaChiGiaoHang,
            TongTien = dh.TongTien,
            NgayDatHang = dh.NgayDatHang,
            ChiTietSanPhams = dh.ChiTietDonHangs.Select(ct => new ChiTietSanPhamTrongDonDto
            {
                TenSanPham = ct.SanPham.TenSanPham,
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia,
                ThanhTien = ct.ThanhTien ?? 0
            }).ToList()
        };
    }
}