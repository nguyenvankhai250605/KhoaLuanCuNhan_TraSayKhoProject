using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IDanhGiaRepository
    {
        Task<List<DanhGium>> GetAllAsync();
        Task<DanhGium?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}