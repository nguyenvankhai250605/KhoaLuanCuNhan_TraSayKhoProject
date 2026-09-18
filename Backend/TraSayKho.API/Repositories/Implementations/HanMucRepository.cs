using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class HanMucRepository : IHanMucRepository
    {
        private readonly TraSayKhoDbContext _context;
        public HanMucRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> SanPhamExistsAsync(int sanPhamId)
            => await _context.SanPhams.AnyAsync(sp => sp.SanPhamId == sanPhamId);

        public async Task<bool> ChiNhanhExistsAsync(int chiNhanhId)
            => await _context.ChiNhanhs.AnyAsync(cn => cn.ChiNhanhId == chiNhanhId);

        public async Task<bool> DaTonTaiAsync(int sanPhamId, int? chiNhanhId)
        {
            return await _context.HanMucSanPhams.AnyAsync(h =>
                h.SanPhamId == sanPhamId && h.ChiNhanhId == chiNhanhId);
        }

        public async Task<List<HanMucSanPham>> GetAllAsync()
        {
            return await _context.HanMucSanPhams
                .Include(h => h.SanPham)
                .Include(h => h.ChiNhanh)
                .ToListAsync();
        }

        public async Task<HanMucSanPham?> GetByIdAsync(int id)
        {
            return await _context.HanMucSanPhams
                .Include(h => h.SanPham)
                .Include(h => h.ChiNhanh)
                .FirstOrDefaultAsync(h => h.HanMucId == id);
        }

        public async Task<HanMucSanPham?> GetApDungChoSanPhamAsync(int sanPhamId, int? chiNhanhId)
        {
            // Ưu tiên hạn mức riêng theo đúng chi nhánh trước, nếu không có mới dùng hạn mức chung (ChiNhanhID = null)
            var hanMucRieng = await _context.HanMucSanPhams
                .FirstOrDefaultAsync(h => h.SanPhamId == sanPhamId && h.ChiNhanhId == chiNhanhId && h.TrangThai);

            if (hanMucRieng != null) return hanMucRieng;

            return await _context.HanMucSanPhams
                .FirstOrDefaultAsync(h => h.SanPhamId == sanPhamId && h.ChiNhanhId == null && h.TrangThai);
        }

        public async Task<HanMucSanPham> AddAsync(HanMucSanPham hanMuc)
        {
            _context.HanMucSanPhams.Add(hanMuc);
            await _context.SaveChangesAsync();
            return hanMuc;
        }

        public async Task<bool> UpdateAsync(int id, HanMucSanPham hanMuc)
        {
            var existing = await _context.HanMucSanPhams.FindAsync(id);
            if (existing == null) return false;

            existing.HanMucBanNgay = hanMuc.HanMucBanNgay;
            existing.HanMucBanTuan = hanMuc.HanMucBanTuan;
            existing.TonKhoToiThieu = hanMuc.TonKhoToiThieu;
            existing.TonKhoToiDa = hanMuc.TonKhoToiDa;
            existing.SoNgayLuuKhoToiDa = hanMuc.SoNgayLuuKhoToiDa;
            existing.TrangThai = hanMuc.TrangThai;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _context.HanMucSanPhams.FindAsync(id);
            if (existing == null) return false;

            existing.TrangThai = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DemSoLuongDaBanTrongKhoangAsync(int sanPhamId, DateTime tuNgay, DateTime denNgay)
        {
            return await _context.ChiTietDonHangs
                .Where(ct => ct.SanPhamId == sanPhamId
                    && ct.DonHang.NgayDatHang >= tuNgay
                    && ct.DonHang.NgayDatHang <= denNgay
                    && ct.DonHang.TrangThai.TenTrangThai != "DaHuy")
                .SumAsync(ct => (int?)ct.SoLuong) ?? 0;
        }

        public async Task<List<SanPham>> GetTonKhoVuotNguongAsync()
        {
            return await _context.SanPhams.ToListAsync();   // lọc chi tiết thực hiện ở Service (cần join với HanMuc)
        }
    }
}