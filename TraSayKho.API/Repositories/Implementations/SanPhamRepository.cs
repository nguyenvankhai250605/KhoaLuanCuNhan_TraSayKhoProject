using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class SanPhamRepository : ISanPhamRepository
    {
        private readonly TraSayKhoDbContext _context;
        public SanPhamRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<SanPham>> GetAllAsync()
        {
            return await _context.SanPhams.Include(sp => sp.DanhMuc).ToListAsync();
        }

        public async Task<SanPham?> GetByIdAsync(int id)
        {
            return await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .FirstOrDefaultAsync(sp => sp.SanPhamId == id);
        }

        public async Task<SanPham> AddAsync(SanPham sanPham)
        {
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();
            // Load lại kèm DanhMuc để trả về đầy đủ thông tin
            await _context.Entry(sanPham).Reference(sp => sp.DanhMuc).LoadAsync();
            return sanPham;
        }

        public async Task<bool> UpdateAsync(SanPham sanPham)
        {
            var existing = await _context.SanPhams.FindAsync(sanPham.SanPhamId);
            if (existing == null) return false;

            existing.TenSanPham = sanPham.TenSanPham;
            existing.DanhMucId = sanPham.DanhMucId;
            existing.MoTaChiTiet = sanPham.MoTaChiTiet;
            existing.XuatXu = sanPham.XuatXu;
            existing.GiaBan = sanPham.GiaBan;
            existing.SoLuongTon = sanPham.SoLuongTon;
            existing.DonViTinh = sanPham.DonViTinh;
            existing.HanSuDung = sanPham.HanSuDung;
            existing.TrangThai = sanPham.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _context.SanPhams.FindAsync(id);
            if (existing == null) return false;

            existing.TrangThai = "NgungBan";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DanhMucExistsAsync(int danhMucId)
        {
            return await _context.DanhMucs.AnyAsync(dm => dm.DanhMucId == danhMucId);
        }
    }
}