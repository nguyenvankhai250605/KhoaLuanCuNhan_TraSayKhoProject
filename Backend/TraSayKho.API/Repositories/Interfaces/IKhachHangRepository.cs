using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IKhachHangRepository
    {
        Task<List<KhachHang>> GetAllAsync();
        Task<KhachHang?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, KhachHang thongTinMoi);
        Task<bool> SetTrangThaiTaiKhoanAsync(int khachHangId, bool trangThai);
    }
}