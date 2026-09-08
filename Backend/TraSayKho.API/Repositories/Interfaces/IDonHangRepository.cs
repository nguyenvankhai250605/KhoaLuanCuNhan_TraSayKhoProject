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

        // ==== CÁC HÀM MỚI PHỤC VỤ TẠO ĐƠN HÀNG ====
        Task<bool> KhachHangExistsAsync(int khachHangId);
        Task<bool> ChiNhanhExistsAsync(int chiNhanhId);
        Task<SanPham?> GetSanPhamAsync(int sanPhamId);
        Task<List<LoHang>> GetLoHangConHangTheoFefoAsync(int sanPhamId, int chiNhanhId);
        Task<KhuyenMai?> GetKhuyenMaiByMaCodeAsync(string maCode);
        Task<DonHang> TaoDonHangAsync(DonHang donHang, List<ChiTietDonHang> chiTiets, List<(LoHang LoHang, int SoLuongTru)> danhSachTruKho, KhuyenMai? khuyenMaiApDung);
    }
}