using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IThongKeService
    {
        Task<List<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime tuNgay, DateTime denNgay, int? chiNhanhId);
        Task<List<SanPhamBanChayDto>> GetTopSanPhamBanChayAsync(DateTime tuNgay, DateTime denNgay, int top, int? chiNhanhId);
        Task<(bool Success, string? ErrorMessage, TongQuanDto? Result)> GetTongQuanAsync(int? chiNhanhId);
    }
}