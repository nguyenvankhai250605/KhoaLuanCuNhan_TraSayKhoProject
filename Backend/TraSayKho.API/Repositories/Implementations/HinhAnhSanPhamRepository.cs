using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class HinhAnhSanPhamRepository : IHinhAnhSanPhamRepository
    {
        private readonly TraSayKhoDbContext _context;
        public HinhAnhSanPhamRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> SanPhamExistsAsync(int sanPhamId)
        {
            return await _context.SanPhams.AnyAsync(sp => sp.SanPhamId == sanPhamId);
        }

        public async Task<List<HinhAnhSanPham>> GetBySanPhamIdAsync(int sanPhamId)
        {
            return await _context.HinhAnhSanPhams
                .Where(ha => ha.SanPhamId == sanPhamId)
                .OrderBy(ha => ha.ThuTuHienThi)
                .ToListAsync();
        }

        public async Task<HinhAnhSanPham?> GetByIdAsync(int id)
        {
            return await _context.HinhAnhSanPhams.FirstOrDefaultAsync(ha => ha.HinhAnhId == id);
        }

        public async Task<HinhAnhSanPham> AddAsync(HinhAnhSanPham hinhAnh)
        {
            _context.HinhAnhSanPhams.Add(hinhAnh);
            await _context.SaveChangesAsync();
            return hinhAnh;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.HinhAnhSanPhams.FindAsync(id);
            if (existing == null) return false;

            _context.HinhAnhSanPhams.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}