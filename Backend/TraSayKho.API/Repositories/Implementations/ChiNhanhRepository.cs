using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class ChiNhanhRepository : IChiNhanhRepository
    {
        private readonly TraSayKhoDbContext _context;
        public ChiNhanhRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<ChiNhanh>> GetAllAsync()
        {
            return await _context.ChiNhanhs.ToListAsync();
        }

        public async Task<ChiNhanh?> GetByIdAsync(int id)
        {
            return await _context.ChiNhanhs.FirstOrDefaultAsync(cn => cn.ChiNhanhId == id);
        }

        public async Task<ChiNhanh> AddAsync(ChiNhanh chiNhanh)
        {
            _context.ChiNhanhs.Add(chiNhanh);
            await _context.SaveChangesAsync();
            return chiNhanh;
        }

        public async Task<bool> UpdateAsync(int id, ChiNhanh chiNhanh)
        {
            var existing = await _context.ChiNhanhs.FindAsync(id);
            if (existing == null) return false;

            existing.TenChiNhanh = chiNhanh.TenChiNhanh;
            existing.DiaChi = chiNhanh.DiaChi;
            existing.SoDienThoai = chiNhanh.SoDienThoai;
            existing.TrangThai = chiNhanh.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _context.ChiNhanhs.FindAsync(id);
            if (existing == null) return false;

            existing.TrangThai = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}