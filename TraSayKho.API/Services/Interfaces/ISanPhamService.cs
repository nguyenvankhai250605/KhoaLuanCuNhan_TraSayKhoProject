using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface ISanPhamService
    {
        Task<List<SanPhamDto>> GetAllAsync();
        Task<SanPhamDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, SanPhamDto? Result)> CreateAsync(SanPhamCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, SanPhamUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}