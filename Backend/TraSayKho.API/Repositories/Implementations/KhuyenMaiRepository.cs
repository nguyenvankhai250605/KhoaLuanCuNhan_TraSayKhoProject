using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class KhuyenMaiRepository : IKhuyenMaiRepository
    {
        private readonly TraSayKhoDbContext _context;
        public KhuyenMaiRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<KhuyenMai>> GetAllAsync()
        {
            return await _context.KhuyenMais.ToListAsync();
        }

        public async Task<KhuyenMai?> GetByIdAsync(int id)
        {
            return await _context.KhuyenMais.FirstOrDefaultAsync(km => km.KhuyenMaiId == id);
        }

        public async Task<bool> MaCodeExistsAsync(string maCode)
        {
            return await _context.KhuyenMais.AnyAsync(km => km.MaCode == maCode);
        }

        public async Task<KhuyenMai> AddAsync(KhuyenMai khuyenMai)
        {
            _context.KhuyenMais.Add(khuyenMai);
            await _context.SaveChangesAsync();
            return khuyenMai;
        }

        public async Task<bool> UpdateAsync(KhuyenMai khuyenMai)
        {
            var existing = await _context.KhuyenMais.FindAsync(khuyenMai.KhuyenMaiId);
            if (existing == null) return false;

            existing.MoTa = khuyenMai.MoTa;
            existing.LoaiGiam = khuyenMai.LoaiGiam;
            existing.GiaTriGiam = khuyenMai.GiaTriGiam;
            existing.GiaTriDonHangToiThieu = khuyenMai.GiaTriDonHangToiThieu;
            existing.NgayBatDau = khuyenMai.NgayBatDau;
            existing.NgayKetThuc = khuyenMai.NgayKetThuc;
            existing.SoLuotSuDungToiDa = khuyenMai.SoLuotSuDungToiDa;
            existing.TrangThai = khuyenMai.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _context.KhuyenMais.FindAsync(id);
            if (existing == null) return false;

            existing.TrangThai = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}