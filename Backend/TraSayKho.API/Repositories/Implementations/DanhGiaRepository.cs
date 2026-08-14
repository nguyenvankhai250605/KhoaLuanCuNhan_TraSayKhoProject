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