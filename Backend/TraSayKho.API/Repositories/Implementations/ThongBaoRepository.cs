using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly TraSayKhoDbContext _context;
        public ThongBaoRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<ThongBao>> GetAllAsync()
        {
            return await _context.ThongBaos
                .Include(tb => tb.KhachHang)
                .OrderByDescending(tb => tb.NgayTao)
                .ToListAsync();
        }

        public async Task<List<ThongBao>> GetByKhachHangIdAsync(int khachHangId)
        {
            return await _context.ThongBaos
                .Include(tb => tb.KhachHang)
                .Where(tb => tb.KhachHangId == khachHangId)
                .OrderByDescending(tb => tb.NgayTao)
                .ToListAsync();
        }

        public async Task<List<int>> GetAllKhachHangIdsAsync()
        {
            return await _context.KhachHangs.Select(kh => kh.KhachHangId).ToListAsync();
        }

        public async Task AddRangeAsync(List<ThongBao> danhSachThongBao)
        {
            _context.ThongBaos.AddRange(danhSachThongBao);
            await _context.SaveChangesAsync();
        }
    }
}