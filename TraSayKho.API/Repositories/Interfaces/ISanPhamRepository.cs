using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface ISanPhamRepository
    {
        Task<List<SanPham>> GetAllAsync();
        Task<SanPham?> GetByIdAsync(int id);
        Task<SanPham> AddAsync(SanPham sanPham);
        Task<bool> UpdateAsync(SanPham sanPham);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> DanhMucExistsAsync(int danhMucId);
    }
}