using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly TraSayKhoDbContext _context;
        public GioHangRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> KhachHangExistsAsync(int khachHangId)
        {
            return await _context.KhachHangs.AnyAsync(kh => kh.KhachHangId == khachHangId);
        }

        public async Task<SanPham?> GetSanPhamAsync(int sanPhamId)
        {
            return await _context.SanPhams.FirstOrDefaultAsync(sp => sp.SanPhamId == sanPhamId);
        }

        public async Task<GioHang> GetOrCreateGioHangAsync(int khachHangId)
        {
            var gioHang = await _context.GioHangs.FirstOrDefaultAsync(gh => gh.KhachHangId == khachHangId);
            if (gioHang != null) return gioHang;

            gioHang = new GioHang { KhachHangId = khachHangId };
            _context.GioHangs.Add(gioHang);
            await _context.SaveChangesAsync();
            return gioHang;
        }

        public async Task<GioHang?> GetGioHangDayDuAsync(int khachHangId)
        {
            return await _context.GioHangs
                .Include(gh => gh.ChiTietGioHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(gh => gh.KhachHangId == khachHangId);
        }

        public async Task<ChiTietGioHang?> TimChiTietTheoSanPhamAsync(int gioHangId, int sanPhamId)
        {
            return await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(ct => ct.GioHangId == gioHangId && ct.SanPhamId == sanPhamId);
        }

        public async Task<ChiTietGioHang?> GetChiTietByIdAsync(int chiTietId)
        {
            return await _context.ChiTietGioHangs
                .Include(ct => ct.SanPham)
                .FirstOrDefaultAsync(ct => ct.ChiTietGioHangId == chiTietId);
        }

        public async Task<ChiTietGioHang> ThemChiTietAsync(ChiTietGioHang chiTiet)
        {
            _context.ChiTietGioHangs.Add(chiTiet);
            await _context.SaveChangesAsync();
            return chiTiet;
        }

        public async Task CapNhatSoLuongAsync(ChiTietGioHang chiTiet, int soLuongMoi)
        {
            chiTiet.SoLuong = soLuongMoi;
            await _context.SaveChangesAsync();
        }

        public async Task XoaChiTietAsync(ChiTietGioHang chiTiet)
        {
            _context.ChiTietGioHangs.Remove(chiTiet);
            await _context.SaveChangesAsync();
        }

        public async Task XoaToanBoGioHangAsync(int gioHangId)
        {
            var chiTiets = _context.ChiTietGioHangs.Where(ct => ct.GioHangId == gioHangId);
            _context.ChiTietGioHangs.RemoveRange(chiTiets);
            await _context.SaveChangesAsync();
        }
    }
}