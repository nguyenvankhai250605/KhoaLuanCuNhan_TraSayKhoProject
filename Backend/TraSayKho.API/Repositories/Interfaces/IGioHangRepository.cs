using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IGioHangRepository
    {
        Task<bool> KhachHangExistsAsync(int khachHangId);
        Task<SanPham?> GetSanPhamAsync(int sanPhamId);
        Task<GioHang> GetOrCreateGioHangAsync(int khachHangId);
        Task<GioHang?> GetGioHangDayDuAsync(int khachHangId);
        Task<ChiTietGioHang?> TimChiTietTheoSanPhamAsync(int gioHangId, int sanPhamId);
        Task<ChiTietGioHang?> GetChiTietByIdAsync(int chiTietId);
        Task<ChiTietGioHang> ThemChiTietAsync(ChiTietGioHang chiTiet);
        Task CapNhatSoLuongAsync(ChiTietGioHang chiTiet, int soLuongMoi);
        Task XoaChiTietAsync(ChiTietGioHang chiTiet);
        Task XoaToanBoGioHangAsync(int gioHangId);
    }
}