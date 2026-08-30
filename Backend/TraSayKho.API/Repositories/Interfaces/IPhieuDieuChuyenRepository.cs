using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IPhieuDieuChuyenRepository
    {
        Task<bool> ChiNhanhExistsAsync(int chiNhanhId);
        Task<bool> NhanVienExistsAsync(int nhanVienId);
        Task<LoHang?> GetLoHangByIdAsync(int loHangId);
        Task<List<PhieuDieuChuyenKho>> GetAllAsync();
        Task<PhieuDieuChuyenKho?> GetByIdAsync(int id);
        Task<PhieuDieuChuyenKho> CreateAsync(PhieuDieuChuyenKho phieu, List<ChiTietPhieuDieuChuyen> chiTiets);
        Task<bool> XacNhanAsync(int phieuId, int nhanVienXacNhanId);
        Task<bool> HuyAsync(int phieuId);
    }
}