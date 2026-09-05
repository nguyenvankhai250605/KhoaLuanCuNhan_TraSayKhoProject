using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IBacGiamGiaService
    {
        Task<List<BacGiamGiaDto>> GetAllAsync();
        Task<(bool Success, string? ErrorMessage, BacGiamGiaDto? Result)> CreateAsync(BacGiamGiaCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, BacGiamGiaUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}