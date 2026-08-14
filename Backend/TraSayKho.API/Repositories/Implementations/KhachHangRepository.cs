using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly TraSayKhoDbContext _context;
        public KhachHangRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<KhachHang>> GetAllAsync()
        {
            return await _context.KhachHangs.Include(kh => kh.TaiKhoan).ToListAsync();
        }

        public async Task<KhachHang?> GetByIdAsync(int id)
        {
            return await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .FirstOrDefaultAsync(kh => kh.KhachHangId == id);
        }

        public async Task<bool> UpdateAsync(int id, KhachHang thongTinMoi)
        {
            var existing = await _context.KhachHangs.FindAsync(id);
            if (existing == null) return false;

            existing.HoTen = thongTinMoi.HoTen;
            existing.DiaChi = thongTinMoi.DiaChi;
            existing.NgaySinh = thongTinMoi.NgaySinh;
            existing.GioiTinh = thongTinMoi.GioiTinh;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetTrangThaiTaiKhoanAsync(int khachHangId, bool trangThai)
        {
            var khachHang = await _context.KhachHangs
                .Include(kh => kh.TaiKhoan)
                .FirstOrDefaultAsync(kh => kh.KhachHangId == khachHangId);

            if (khachHang == null) return false;

            khachHang.TaiKhoan.TrangThai = trangThai;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}