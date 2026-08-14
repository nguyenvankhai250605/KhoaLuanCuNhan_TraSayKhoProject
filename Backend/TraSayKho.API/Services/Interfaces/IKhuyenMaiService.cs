using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IKhuyenMaiService
    {
        Task<List<KhuyenMaiDto>> GetAllAsync();
        Task<KhuyenMaiDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, KhuyenMaiDto? Result)> CreateAsync(KhuyenMaiCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, KhuyenMaiUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}