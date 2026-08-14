using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class DanhMucRepository : IDanhMucRepository
    {
        private readonly TraSayKhoDbContext _context;
        public DanhMucRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<DanhMuc>> GetAllAsync()
        {
            return await _context.DanhMucs.ToListAsync();
        }

        public async Task<DanhMuc?> GetByIdAsync(int id)
        {
            return await _context.DanhMucs.FirstOrDefaultAsync(dm => dm.DanhMucId == id);
        }

        public async Task<DanhMuc> AddAsync(DanhMuc danhMuc)
        {
            _context.DanhMucs.Add(danhMuc);
            await _context.SaveChangesAsync();
            return danhMuc;
        }

        public async Task<bool> UpdateAsync(DanhMuc danhMuc)
        {
            var existing = await _context.DanhMucs.FindAsync(danhMuc.DanhMucId);
            if (existing == null) return false;

            existing.TenDanhMuc = danhMuc.TenDanhMuc;
            existing.MoTa = danhMuc.MoTa;
            existing.TrangThai = danhMuc.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _context.DanhMucs.FindAsync(id);
            if (existing == null) return false;

            existing.TrangThai = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}