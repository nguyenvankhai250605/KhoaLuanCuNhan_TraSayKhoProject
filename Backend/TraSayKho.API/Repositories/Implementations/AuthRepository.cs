using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly TraSayKhoDbContext _context;
        public AuthRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> TenDangNhapExistsAsync(string tenDangNhap)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.TenDangNhap == tenDangNhap);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.TaiKhoans.AnyAsync(tk => tk.Email == email);
        }

        public async Task<bool> ChiNhanhExistsAsync(int chiNhanhId)
        {
            return await _context.ChiNhanhs.AnyAsync(cn => cn.ChiNhanhId == chiNhanhId);
        }

        public async Task<int?> GetVaiTroIdAsync(string tenVaiTro)
        {
            var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(vt => vt.TenVaiTro == tenVaiTro);
            return vaiTro?.VaiTroId;
        }

        public async Task<TaiKhoan> DangKyKhachHangAsync(TaiKhoan taiKhoan, KhachHang khachHang)
        {
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();   // lưu trước để có TaiKhoanId

            khachHang.TaiKhoanId = taiKhoan.TaiKhoanId;
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            return taiKhoan;
        }

        public async Task<TaiKhoan> TaoNhanVienAsync(TaiKhoan taiKhoan, NhanVien nhanVien)
        {
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            nhanVien.TaiKhoanId = taiKhoan.TaiKhoanId;
            _context.NhanViens.Add(nhanVien);
            await _context.SaveChangesAsync();

            return taiKhoan;
        }

        public async Task<TaiKhoan?> GetTaiKhoanDayDuAsync(string tenDangNhap)
        {
            return await _context.TaiKhoans
                .Include(tk => tk.VaiTro)
                .Include(tk => tk.KhachHang)
                .Include(tk => tk.NhanVien)
                    .ThenInclude(nv => nv!.ChiNhanh)
                .FirstOrDefaultAsync(tk => tk.TenDangNhap == tenDangNhap);
        }
    }
}