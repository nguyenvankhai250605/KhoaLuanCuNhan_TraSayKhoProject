using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IKhuyenMaiRepository
    {
        Task<List<KhuyenMai>> GetAllAsync();
        Task<KhuyenMai?> GetByIdAsync(int id);
        Task<bool> MaCodeExistsAsync(string maCode);
        Task<KhuyenMai> AddAsync(KhuyenMai khuyenMai);
        Task<bool> UpdateAsync(KhuyenMai khuyenMai);
        Task<bool> SoftDeleteAsync(int id);
    }
}