using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IDonHangService
    {
        Task<List<DonHangDto>> GetAllAsync();
        Task<DonHangChiTietDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int id, CapNhatTrangThaiDto dto);
    }
}