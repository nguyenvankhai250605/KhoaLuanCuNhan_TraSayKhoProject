using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IThongKeService
    {
        Task<List<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime tuNgay, DateTime denNgay);
        Task<List<SanPhamBanChayDto>> GetTopSanPhamBanChayAsync(DateTime tuNgay, DateTime denNgay, int top);
        Task<TongQuanDto> GetTongQuanAsync();
    }
}