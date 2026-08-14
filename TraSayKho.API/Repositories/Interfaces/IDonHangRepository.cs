using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IDonHangRepository
    {
        Task<List<DonHang>> GetAllAsync();
        Task<DonHang?> GetByIdWithDetailsAsync(int id);
        Task<DonHang?> GetByIdAsync(int id);
        Task<TrangThaiDonHang?> GetTrangThaiByTenAsync(string tenTrangThai);
        Task<bool> UpdateTrangThaiAsync(int donHangId, int trangThaiIdMoi);
    }
}