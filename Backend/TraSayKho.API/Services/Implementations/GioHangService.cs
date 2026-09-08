using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class GioHangService : IGioHangService
    {
        private readonly IGioHangRepository _repository;
        public GioHangService(IGioHangRepository repository) => _repository = repository;

        public async Task<(bool Success, string? ErrorMessage, GioHangDto? Result)> GetGioHangAsync(int khachHangId)
        {
            if (!await _repository.KhachHangExistsAsync(khachHangId))
                return (false, "Khách hàng không tồn tại.", null);

            var gioHang = await _repository.GetGioHangDayDuAsync(khachHangId);

            if (gioHang == null)
            {
                // Chưa từng có giỏ hàng, tạo mới rỗng luôn cho tiện, nhưng chưa cần lưu chi tiết
                var moi = await _repository.GetOrCreateGioHangAsync(khachHangId);
                return (true, null, new GioHangDto { GioHangId = moi.GioHangId, KhachHangId = khachHangId });
            }

            return (true, null, MapToDto(gioHang));
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemVaoGioHangAsync(int khachHangId, ThemVaoGioHangDto dto)
        {
            if (!await _repository.KhachHangExistsAsync(khachHangId))
                return (false, "Khách hàng không tồn tại.");

            var sanPham = await _repository.GetSanPhamAsync(dto.SanPhamId);
            if (sanPham == null)
                return (false, "Sản phẩm không tồn tại.");

            if (sanPham.TrangThai != "DangBan")
                return (false, "Sản phẩm hiện không còn bán.");

            if (dto.SoLuong <= 0)
                return (false, "Số lượng phải lớn hơn 0.");

            var gioHang = await _repository.GetOrCreateGioHangAsync(khachHangId);
            var chiTietCu = await _repository.TimChiTietTheoSanPhamAsync(gioHang.GioHangId, dto.SanPhamId);

            if (chiTietCu != null)
            {
                // Sản phẩm đã có trong giỏ → cộng dồn số lượng, không tạo dòng mới
                await _repository.CapNhatSoLuongAsync(chiTietCu, chiTietCu.SoLuong + dto.SoLuong);
            }
            else
            {
                await _repository.ThemChiTietAsync(new ChiTietGioHang
                {
                    GioHangId = gioHang.GioHangId,
                    SanPhamId = dto.SanPhamId,
                    SoLuong = dto.SoLuong,
                    NgayThem = DateTime.Now
                });
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatSoLuongAsync(int chiTietId, CapNhatSoLuongGioHangDto dto)
        {
            if (dto.SoLuong <= 0)
                return (false, "Số lượng phải lớn hơn 0. Nếu muốn xóa, dùng API xóa riêng.");

            var chiTiet = await _repository.GetChiTietByIdAsync(chiTietId);
            if (chiTiet == null)
                return (false, "Không tìm thấy sản phẩm trong giỏ hàng.");

            await _repository.CapNhatSoLuongAsync(chiTiet, dto.SoLuong);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaChiTietAsync(int chiTietId)
        {
            var chiTiet = await _repository.GetChiTietByIdAsync(chiTietId);
            if (chiTiet == null)
                return (false, "Không tìm thấy sản phẩm trong giỏ hàng.");

            await _repository.XoaChiTietAsync(chiTiet);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaToanBoAsync(int khachHangId)
        {
            if (!await _repository.KhachHangExistsAsync(khachHangId))
                return (false, "Khách hàng không tồn tại.");

            var gioHang = await _repository.GetOrCreateGioHangAsync(khachHangId);
            await _repository.XoaToanBoGioHangAsync(gioHang.GioHangId);
            return (true, null);
        }

        private static GioHangDto MapToDto(GioHang gioHang)
        {
            var chiTietDto = gioHang.ChiTietGioHangs.Select(ct => new ChiTietGioHangDto
            {
                ChiTietGioHangId = ct.ChiTietGioHangId,
                SanPhamId = ct.SanPhamId,
                TenSanPham = ct.SanPham.TenSanPham,
                GiaBan = ct.SanPham.GiaBan,
                SoLuong = ct.SoLuong,
                ThanhTien = ct.SanPham.GiaBan * ct.SoLuong,
                ConHang = ct.SanPham.TrangThai == "DangBan"
            }).ToList();

            return new GioHangDto
            {
                GioHangId = gioHang.GioHangId,
                KhachHangId = gioHang.KhachHangId,
                ChiTiet = chiTietDto,
                TongTienTamTinh = chiTietDto.Sum(ct => ct.ThanhTien)
            };
        }
    }
}