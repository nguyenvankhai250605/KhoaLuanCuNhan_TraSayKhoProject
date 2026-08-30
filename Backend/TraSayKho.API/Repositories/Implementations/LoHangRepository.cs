using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class LoHangRepository : ILoHangRepository
    {
        private readonly TraSayKhoDbContext _context;
        public LoHangRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> SanPhamExistsAsync(int sanPhamId)
        {
            return await _context.SanPhams.AnyAsync(sp => sp.SanPhamId == sanPhamId);
        }

        public async Task<bool> ChiNhanhExistsAsync(int chiNhanhId)
        {
            return await _context.ChiNhanhs.AnyAsync(cn => cn.ChiNhanhId == chiNhanhId);
        }

        public async Task<List<LoHang>> GetAllAsync()
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ChiNhanh)
                .OrderBy(lh => lh.HanSuDung)   // mặc định sắp theo FEFO luôn cho tiện nhìn
                .ToListAsync();
        }

        public async Task<LoHang?> GetByIdAsync(int id)
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ChiNhanh)
                .FirstOrDefaultAsync(lh => lh.LoHangId == id);
        }

        public async Task<List<LoHang>> GetBySanPhamAsync(int sanPhamId)
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ChiNhanh)
                .Where(lh => lh.SanPhamId == sanPhamId)
                .OrderBy(lh => lh.HanSuDung)
                .ToListAsync();
        }

        public async Task<List<LoHang>> GetSapHetHanAsync(int soNgayNguong)
        {
            var ngayNguong = DateOnly.FromDateTime(DateTime.Now.AddDays(soNgayNguong));

            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ChiNhanh)
                .Where(lh => lh.TrangThai == "ConHang" && lh.HanSuDung <= ngayNguong)
                .OrderBy(lh => lh.HanSuDung)
                .ToListAsync();
        }

        public async Task<LoHang> AddAsync(LoHang loHang)
        {
            _context.LoHangs.Add(loHang);
            await _context.SaveChangesAsync();

            await _context.Entry(loHang).Reference(lh => lh.SanPham).LoadAsync();
            await _context.Entry(loHang).Reference(lh => lh.ChiNhanh).LoadAsync();

            return loHang;
        }

        public async Task<bool> UpdateXaKhoAsync(int loHangId, decimal? mucGiam, DateOnly? tuNgay, DateOnly? denNgay)
        {
            var loHang = await _context.LoHangs.FindAsync(loHangId);
            if (loHang == null) return false;

            loHang.MucGiamGiaHienTai = mucGiam;
            loHang.NgayBatDauApDungGiam = tuNgay;
            loHang.NgayKetThucApDungGiam = denNgay;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DongBoTonKhoSanPhamAsync(int sanPhamId)
        {
            var sanPham = await _context.SanPhams.FindAsync(sanPhamId);
            if (sanPham == null) return;

            var loConHang = await _context.LoHangs
                .Where(lh => lh.SanPhamId == sanPhamId && lh.TrangThai == "ConHang")
                .ToListAsync();

            sanPham.SoLuongTon = loConHang.Sum(lh => lh.SoLuongConLai);
            sanPham.HanSuDung = loConHang.Any() ? loConHang.Min(lh => lh.HanSuDung) : null;

            await _context.SaveChangesAsync();
        }
    }
}