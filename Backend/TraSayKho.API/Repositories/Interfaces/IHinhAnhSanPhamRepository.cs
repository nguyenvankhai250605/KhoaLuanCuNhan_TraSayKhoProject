using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IHinhAnhSanPhamRepository
    {
        Task<bool> SanPhamExistsAsync(int sanPhamId);
        Task<List<HinhAnhSanPham>> GetBySanPhamIdAsync(int sanPhamId);
        Task<HinhAnhSanPham?> GetByIdAsync(int id);
        Task<HinhAnhSanPham> AddAsync(HinhAnhSanPham hinhAnh);
        Task<bool> DeleteAsync(int id);
    }
}