using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IDanhMucService
    {
        Task<List<DanhMucDto>> GetAllAsync();
        Task<DanhMucDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, DanhMucDto? Result)> CreateAsync(DanhMucCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, DanhMucUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}