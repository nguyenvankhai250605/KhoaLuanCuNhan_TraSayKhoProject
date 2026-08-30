using TraSayKho.API.DTOs;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class ThongKeService : IThongKeService
    {
        private readonly IThongKeRepository _repository;
        public ThongKeService(IThongKeRepository repository) => _repository = repository;

        public async Task<List<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime tuNgay, DateTime denNgay, int? chiNhanhId)
        {
            var donHangs = await _repository.GetDonHangHoanThanhTrongKhoangAsync(tuNgay, denNgay, chiNhanhId);

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

        public async Task<List<SanPhamBanChayDto>> GetTopSanPhamBanChayAsync(DateTime tuNgay, DateTime denNgay, int top, int? chiNhanhId)
        {
            var chiTiets = await _repository.GetChiTietDonHangHoanThanhTrongKhoangAsync(tuNgay, denNgay, chiNhanhId);

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

        public async Task<(bool Success, string? ErrorMessage, TongQuanDto? Result)> GetTongQuanAsync(int? chiNhanhId)
        {
            string phamViBaoCao = "Toàn hệ thống";

            if (chiNhanhId.HasValue)
            {
                var tenChiNhanh = await _repository.GetTenChiNhanhAsync(chiNhanhId.Value);
                if (tenChiNhanh == null)
                    return (false, "Chi nhánh không tồn tại.", null);

                phamViBaoCao = tenChiNhanh;
            }

            var tuNgayXaXua = new DateTime(2000, 1, 1);
            var denNgayHienTai = DateTime.Now;

            var donHangHoanThanh = await _repository.GetDonHangHoanThanhTrongKhoangAsync(tuNgayXaXua, denNgayHienTai, chiNhanhId);
            var donHangChoXuLy = await _repository.DemDonHangTheoTrangThaiAsync(
                new[] { "ChoXacNhan", "DangXuLy", "DangGiao" }, chiNhanhId);

            // Số khách hàng và số sản phẩm đang bán là số liệu toàn hệ thống (không tách theo chi nhánh,
            // vì đây là danh mục chung, không phải dữ liệu tồn kho riêng từng chi nhánh)
            var tongKhachHang = await _repository.DemTongKhachHangAsync();
            var tongSanPhamDangBan = await _repository.DemTongSanPhamDangBanAsync();

            var result = new TongQuanDto
            {
                ChiNhanhId = chiNhanhId,
                PhamViBaoCao = phamViBaoCao,
                TongDoanhThu = donHangHoanThanh.Sum(dh => dh.TongTien),
                TongDonHangHoanThanh = donHangHoanThanh.Count,
                DonHangChoXuLy = donHangChoXuLy,
                TongKhachHang = tongKhachHang,
                TongSanPhamDangBan = tongSanPhamDangBan
            };

            return (true, null, result);
        }
    }
}