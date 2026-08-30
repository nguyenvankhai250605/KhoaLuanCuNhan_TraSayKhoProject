using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class ThongKeRepository : IThongKeRepository
    {
        private readonly TraSayKhoDbContext _context;
        public ThongKeRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<DonHang>> GetDonHangHoanThanhTrongKhoangAsync(DateTime tuNgay, DateTime denNgay, int? chiNhanhId)
        {
            var query = _context.DonHangs
                .Include(dh => dh.TrangThai)
                .Where(dh => dh.TrangThai.TenTrangThai == "HoanThanh"
                          && dh.NgayDatHang >= tuNgay
                          && dh.NgayDatHang <= denNgay);

            if (chiNhanhId.HasValue)
                query = query.Where(dh => dh.ChiNhanhId == chiNhanhId.Value);

            return await query.ToListAsync();
        }

        public async Task<List<ChiTietDonHang>> GetChiTietDonHangHoanThanhTrongKhoangAsync(DateTime tuNgay, DateTime denNgay, int? chiNhanhId)
        {
            var query = _context.ChiTietDonHangs
                .Include(ct => ct.SanPham)
                .Include(ct => ct.DonHang)
                    .ThenInclude(dh => dh.TrangThai)
                .Where(ct => ct.DonHang.TrangThai.TenTrangThai == "HoanThanh"
                          && ct.DonHang.NgayDatHang >= tuNgay
                          && ct.DonHang.NgayDatHang <= denNgay);

            if (chiNhanhId.HasValue)
                query = query.Where(ct => ct.DonHang.ChiNhanhId == chiNhanhId.Value);

            return await query.ToListAsync();
        }

        public async Task<int> DemTongKhachHangAsync()
        {
            return await _context.KhachHangs.CountAsync();
        }

        public async Task<int> DemTongSanPhamDangBanAsync()
        {
            return await _context.SanPhams.CountAsync(sp => sp.TrangThai == "DangBan");
        }

        public async Task<int> DemDonHangTheoTrangThaiAsync(string[] cacTenTrangThai, int? chiNhanhId)
        {
            var query = _context.DonHangs
                .Include(dh => dh.TrangThai)
                .Where(dh => cacTenTrangThai.Contains(dh.TrangThai.TenTrangThai));

            if (chiNhanhId.HasValue)
                query = query.Where(dh => dh.ChiNhanhId == chiNhanhId.Value);

            return await query.CountAsync();
        }

        public async Task<string?> GetTenChiNhanhAsync(int chiNhanhId)
        {
            var chiNhanh = await _context.ChiNhanhs.FindAsync(chiNhanhId);
            return chiNhanh?.TenChiNhanh;
        }
    }
}