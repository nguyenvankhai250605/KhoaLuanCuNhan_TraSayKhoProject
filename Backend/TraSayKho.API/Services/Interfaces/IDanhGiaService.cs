using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IDanhGiaService
    {
        Task<List<DanhGiaDto>> GetAllAsync();
        Task<DanhGiaDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}