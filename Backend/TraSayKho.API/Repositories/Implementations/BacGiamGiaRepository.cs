using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class BacGiamGiaRepository : IBacGiamGiaRepository
    {
        private readonly TraSayKhoDbContext _context;
        public BacGiamGiaRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<BacGiamGiaXaKho>> GetAllAsync()
        {
            return await _context.BacGiamGiaXaKhos
                .Include(b => b.DanhMuc)
                .OrderBy(b => b.DanhMucId)
                .ThenBy(b => b.PhanTramThoiGianConLaiToiDa)
                .ToListAsync();
        }

        public async Task<List<BacGiamGiaXaKho>> GetDangHoatDongAsync()
        {
            return await _context.BacGiamGiaXaKhos
                .Where(b => b.TrangThai)
                .OrderBy(b => b.PhanTramThoiGianConLaiToiDa)   // % nhỏ nhất (gấp nhất) đứng trước
                .ToListAsync();
        }

        public async Task<BacGiamGiaXaKho?> GetByIdAsync(int id)
        {
            return await _context.BacGiamGiaXaKhos.FirstOrDefaultAsync(b => b.BacGiamGiaId == id);
        }

        public async Task<bool> DanhMucExistsAsync(int danhMucId)
        {
            return await _context.DanhMucs.AnyAsync(dm => dm.DanhMucId == danhMucId);
        }

        public async Task<BacGiamGiaXaKho> AddAsync(BacGiamGiaXaKho bac)
        {
            _context.BacGiamGiaXaKhos.Add(bac);
            await _context.SaveChangesAsync();
            return bac;
        }

        public async Task<bool> UpdateAsync(int id, BacGiamGiaXaKho bac)
        {
            var existing = await _context.BacGiamGiaXaKhos.FindAsync(id);
            if (existing == null) return false;

            existing.DanhMucId = bac.DanhMucId;
            existing.TenBac = bac.TenBac;
            existing.PhanTramThoiGianConLaiToiDa = bac.PhanTramThoiGianConLaiToiDa;
            existing.MucGiamGiaPhanTram = bac.MucGiamGiaPhanTram;
            existing.TrangThai = bac.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _context.BacGiamGiaXaKhos.FindAsync(id);
            if (existing == null) return false;

            existing.TrangThai = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}