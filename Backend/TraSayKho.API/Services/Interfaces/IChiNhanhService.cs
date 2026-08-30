using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IChiNhanhService
    {
        Task<List<ChiNhanhDto>> GetAllAsync();
        Task<ChiNhanhDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, ChiNhanhDto? Result)> CreateAsync(ChiNhanhCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, ChiNhanhUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}