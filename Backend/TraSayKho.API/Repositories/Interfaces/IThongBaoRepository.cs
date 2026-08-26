using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IThongBaoRepository
    {
        Task<List<ThongBao>> GetAllAsync();
        Task<List<ThongBao>> GetByKhachHangIdAsync(int khachHangId);
        Task<List<int>> GetAllKhachHangIdsAsync();
        Task AddRangeAsync(List<ThongBao> danhSachThongBao);
    }
}