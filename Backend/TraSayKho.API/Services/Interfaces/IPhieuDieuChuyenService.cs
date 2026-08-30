using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IPhieuDieuChuyenService
    {
        Task<List<PhieuDieuChuyenDto>> GetAllAsync();
        Task<PhieuDieuChuyenDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, PhieuDieuChuyenDto? Result)> CreateAsync(PhieuDieuChuyenCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> XacNhanAsync(int id, XacNhanPhieuDto dto);
        Task<(bool Success, string? ErrorMessage)> HuyAsync(int id);
    }
}