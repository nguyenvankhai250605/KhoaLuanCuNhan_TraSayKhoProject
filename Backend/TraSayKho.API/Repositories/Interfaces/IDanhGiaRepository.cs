using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IDanhGiaRepository
    {
        Task<List<DanhGium>> GetAllAsync();
        Task<DanhGium?> GetByIdAsync(int id);
        Task<DonHang?> GetDonHangAsync(int donHangId);
        Task<bool> ExistsAsync(int donHangId, int sanPhamId);
        Task<DanhGium> CreateAsync(DanhGium danhGia);
        Task<bool> DeleteAsync(int id);
    }
}
