using TraSayKho.API.DTOs;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class ThongKeService : IThongKeService
    {
        private readonly IThongKeRepository _repository;
        public ThongKeService(IThongKeRepository repository) => _repository = repository;

        public async Task<List<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime tuNgay, DateTime denNgay)
        {
            var donHangs = await _repository.GetDonHangHoanThanhTrongKhoangAsync(tuNgay, denNgay);

            return donHangs
                .GroupBy(dh => DateOnly.FromDateTime(dh.NgayDatHang))
                .Select(g => new DoanhThuTheoNgayDto
                {
                    Ngay = g.Key,
                    TongDoanhThu = g.Sum(dh => dh.TongTien),
                    SoDonHang = g.Count()
                })
                .OrderBy(x => x.Ngay)
                .ToList();
        }

        public async Task<List<SanPhamBanChayDto>> GetTopSanPhamBanChayAsync(DateTime tuNgay, DateTime denNgay, int top)
        {
            var chiTiets = await _repository.GetChiTietDonHangHoanThanhTrongKhoangAsync(tuNgay, denNgay);

            return chiTiets
                .GroupBy(ct => new { ct.SanPhamId, ct.SanPham.TenSanPham })
                .Select(g => new SanPhamBanChayDto
                {
                    SanPhamId = g.Key.SanPhamId,
                    TenSanPham = g.Key.TenSanPham,
                    TongSoLuongBan = g.Sum(ct => ct.SoLuong),
                    TongDoanhThu = g.Sum(ct => ct.ThanhTien) ?? 0
                })
                .OrderByDescending(x => x.TongSoLuongBan)
                .Take(top)
                .ToList();
        }

        public async Task<TongQuanDto> GetTongQuanAsync()
        {
            // Lấy toàn bộ lịch sử (không giới hạn ngày) cho phần tổng quan
            var tuNgayXaXua = new DateTime(2000, 1, 1);
            var denNgayHienTai = DateTime.Now;

            var donHangHoanThanh = await _repository.GetDonHangHoanThanhTrongKhoangAsync(tuNgayXaXua, denNgayHienTai);
            var tongKhachHang = await _repository.DemTongKhachHangAsync();
            var tongSanPhamDangBan = await _repository.DemTongSanPhamDangBanAsync();
            var donHangChoXuLy = await _repository.DemDonHangTheoTrangThaiAsync(new[] { "ChoXacNhan", "DangXuLy", "DangGiao" });

            return new TongQuanDto
            {
                TongDoanhThu = donHangHoanThanh.Sum(dh => dh.TongTien),
                TongDonHangHoanThanh = donHangHoanThanh.Count,
                DonHangChoXuLy = donHangChoXuLy,
                TongKhachHang = tongKhachHang,
                TongSanPhamDangBan = tongSanPhamDangBan
            };
        }
    }
}