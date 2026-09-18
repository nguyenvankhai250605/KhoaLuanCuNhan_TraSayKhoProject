using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class DanhGiaRepository : IDanhGiaRepository
    {
        private readonly TraSayKhoDbContext _context;
        public DanhGiaRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<DanhGium>> GetAllAsync()
        {
            return await _context.DanhGia
                .Include(dg => dg.SanPham)
                .Include(dg => dg.KhachHang)
                .ToListAsync();
        }

        public async Task<DanhGium?> GetByIdAsync(int id)
        {
            return await _context.DanhGia
                .Include(dg => dg.SanPham)
                .Include(dg => dg.KhachHang)
                .FirstOrDefaultAsync(dg => dg.DanhGiaId == id);
        }

        public async Task<DonHang?> GetDonHangAsync(int donHangId)
        {
            return await _context.DonHangs
                .Include(dh => dh.TrangThai)
                .Include(dh => dh.ChiTietDonHangs)
                .FirstOrDefaultAsync(dh => dh.DonHangId == donHangId);
        }

        public Task<bool> ExistsAsync(int donHangId, int sanPhamId)
        {
            return _context.DanhGia.AnyAsync(dg =>
                dg.DonHangId == donHangId && dg.SanPhamId == sanPhamId);
        }

        public async Task<DanhGium> CreateAsync(DanhGium danhGia)
        {
            _context.DanhGia.Add(danhGia);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(danhGia.DanhGiaId))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.DanhGia.FindAsync(id);
            if (existing == null) return false;

            _context.DanhGia.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
