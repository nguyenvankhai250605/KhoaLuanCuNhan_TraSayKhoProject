using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface ILoHangRepository
    {
        Task<bool> SanPhamExistsAsync(int sanPhamId);

        Task<bool> SoLoExistsAsync(string soLo);

        Task<List<LoHang>> GetAllAsync();

        Task<LoHang?> GetByIdAsync(int id);

        Task<List<LoHang>> GetBySanPhamAsync(int sanPhamId);

        Task<List<LoHang>> GetSapHetHanAsync();

        Task<LoHang> TaoLoVaPhanBoAsync(
            LoHang loHang,
            int chiNhanhChinhId,
            int soLuongThung,
            int soDonViMoiThung);

        Task DongBoTonKhoSanPhamAsync(int sanPhamId);

        Task<DonViSanPham?>
            GetDonViSanPhamByMaAsync(string maDonVi);

        Task<ChiNhanh?> GetTruSoChinhAsync();
    }
}