using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IThongKeRepository
    {
        Task<List<DonHang>> GetDonHangHoanThanhTrongKhoangAsync(DateTime tuNgay, DateTime denNgay);
        Task<List<ChiTietDonHang>> GetChiTietDonHangHoanThanhTrongKhoangAsync(DateTime tuNgay, DateTime denNgay);
        Task<int> DemTongKhachHangAsync();
        Task<int> DemTongSanPhamDangBanAsync();
        Task<int> DemDonHangTheoTrangThaiAsync(string[] cacTenTrangThai);
    }
}