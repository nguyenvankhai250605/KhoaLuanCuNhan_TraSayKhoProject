using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IHanMucRepository
    {
        Task<bool> SanPhamExistsAsync(int sanPhamId);
        Task<bool> ChiNhanhExistsAsync(int chiNhanhId);
        Task<bool> DaTonTaiAsync(int sanPhamId, int? chiNhanhId);
        Task<List<HanMucSanPham>> GetAllAsync();
        Task<HanMucSanPham?> GetByIdAsync(int id);
        Task<HanMucSanPham?> GetApDungChoSanPhamAsync(int sanPhamId, int? chiNhanhId);
        Task<HanMucSanPham> AddAsync(HanMucSanPham hanMuc);
        Task<bool> UpdateAsync(int id, HanMucSanPham hanMuc);
        Task<bool> SoftDeleteAsync(int id);
        Task<int> DemSoLuongDaBanTrongKhoangAsync(int sanPhamId, DateTime tuNgay, DateTime denNgay);
        Task<List<SanPham>> GetTonKhoVuotNguongAsync();
    }
}