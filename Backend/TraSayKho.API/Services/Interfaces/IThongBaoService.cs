using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IThongBaoService
    {
        Task<List<ThongBaoDto>> GetAllAsync();
        Task<List<ThongBaoDto>> GetByKhachHangIdAsync(int khachHangId);
        Task<(bool Success, string? ErrorMessage, int SoLuongDaGui)> CreateAsync(ThongBaoCreateDto dto);
    }
}