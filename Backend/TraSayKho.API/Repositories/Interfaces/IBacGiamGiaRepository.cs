using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IBacGiamGiaRepository
    {
        Task<List<BacGiamGiaXaKho>> GetAllAsync();
        Task<List<BacGiamGiaXaKho>> GetDangHoatDongAsync();
        Task<BacGiamGiaXaKho?> GetByIdAsync(int id);
        Task<BacGiamGiaXaKho> AddAsync(BacGiamGiaXaKho bac);
        Task<bool> UpdateAsync(int id, BacGiamGiaXaKho bac);
        Task<bool> SoftDeleteAsync(int id);
    }
}