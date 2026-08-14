using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IDanhMucRepository
    {
        Task<List<DanhMuc>> GetAllAsync();
        Task<DanhMuc?> GetByIdAsync(int id);
        Task<DanhMuc> AddAsync(DanhMuc danhMuc);
        Task<bool> UpdateAsync(DanhMuc danhMuc);
        Task<bool> SoftDeleteAsync(int id);
    }
}