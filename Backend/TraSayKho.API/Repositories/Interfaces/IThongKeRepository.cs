using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IThongKeRepository
    {
        Task<List<DonHang>> GetDonHangHoanThanhTrongKhoangAsync(DateTime tuNgay, DateTime denNgay, int? chiNhanhId);
        Task<List<ChiTietDonHang>> GetChiTietDonHangHoanThanhTrongKhoangAsync(DateTime tuNgay, DateTime denNgay, int? chiNhanhId);
        Task<int> DemTongKhachHangAsync();
        Task<int> DemTongSanPhamDangBanAsync();
        Task<int> DemDonHangTheoTrangThaiAsync(string[] cacTenTrangThai, int? chiNhanhId);
        Task<string?> GetTenChiNhanhAsync(int chiNhanhId);
    }
}