using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IChiNhanhRepository
    {
        Task<List<ChiNhanh>> GetAllAsync();
        Task<ChiNhanh?> GetByIdAsync(int id);
        Task<ChiNhanh> AddAsync(ChiNhanh chiNhanh);
        Task<bool> UpdateAsync(int id, ChiNhanh chiNhanh);
        Task<bool> SoftDeleteAsync(int id);
    }
}