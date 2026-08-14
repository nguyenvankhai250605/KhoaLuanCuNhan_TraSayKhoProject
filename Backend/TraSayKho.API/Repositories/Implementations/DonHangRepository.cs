using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly TraSayKhoDbContext _context;
        public DonHangRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<DonHang>> GetAllAsync()
        {
            return await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.TrangThai)
                .ToListAsync();
        }

        public async Task<DonHang?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.TrangThai)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(dh => dh.DonHangId == id);
        }

        public async Task<DonHang?> GetByIdAsync(int id)
        {
            return await _context.DonHangs.FirstOrDefaultAsync(dh => dh.DonHangId == id);
        }

        public async Task<TrangThaiDonHang?> GetTrangThaiByTenAsync(string tenTrangThai)
        {
            return await _context.TrangThaiDonHangs
                .FirstOrDefaultAsync(tt => tt.TenTrangThai == tenTrangThai);
        }

        public async Task<bool> UpdateTrangThaiAsync(int donHangId, int trangThaiIdMoi)
        {
            var donHang = await _context.DonHangs.FindAsync(donHangId);
            if (donHang == null) return false;

            donHang.TrangThaiId = trangThaiIdMoi;

            // Ghi log lịch sử thay đổi trạng thái
            _context.LichSuTrangThaiDonHangs.Add(new LichSuTrangThaiDonHang
            {
                DonHangId = donHangId,
                TrangThaiId = trangThaiIdMoi,
                ThoiGianCapNhat = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}