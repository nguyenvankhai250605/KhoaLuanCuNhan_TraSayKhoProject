using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> TenDangNhapExistsAsync(string tenDangNhap);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ChiNhanhExistsAsync(int chiNhanhId);
        Task<int?> GetVaiTroIdAsync(string tenVaiTro);
        Task<TaiKhoan> DangKyKhachHangAsync(TaiKhoan taiKhoan, KhachHang khachHang);
        Task<TaiKhoan> TaoNhanVienAsync(TaiKhoan taiKhoan, NhanVien nhanVien);
        Task<TaiKhoan?> GetTaiKhoanDayDuAsync(string tenDangNhap);
    }
}