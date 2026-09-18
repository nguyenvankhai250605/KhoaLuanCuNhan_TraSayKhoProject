using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IDonHangRepository
    {
        Task<List<DonHang>> GetAllAsync();

        Task<DonHang?> GetByIdWithDetailsAsync(int id);

        Task<DonHang?> GetByIdAsync(int id);

        Task<TrangThaiDonHang?> GetTrangThaiByTenAsync(
            string tenTrangThai);

        Task<bool> UpdateTrangThaiAsync(
            int donHangId,
            int trangThaiIdMoi);

        // ===== PHỤC VỤ TẠO ĐƠN HÀNG =====

        Task<bool> KhachHangExistsAsync(
            int khachHangId);

        Task<bool> ChiNhanhExistsAsync(
            int chiNhanhId);

        Task<SanPham?> GetSanPhamAsync(
            int sanPhamId);

        // Lấy các đơn vị sản phẩm (hộp hoặc gói) còn trong kho
        // tại đúng chi nhánh và sắp xếp FEFO theo hạn sử dụng của lô.
        Task<List<DonViSanPham>> GetDonViSanPhamConKhoTheoFefoAsync(
            int sanPhamId,
            int chiNhanhId);

        Task<KhuyenMai?> GetKhuyenMaiByMaCodeAsync(
            string maCode);

        Task<DonHang> TaoDonHangAsync(
            DonHang donHang,
            List<ChiTietDonHang> chiTiets,
            Dictionary<ChiTietDonHang, List<DonViSanPham>> danhSachDonViBan,
            KhuyenMai? khuyenMaiApDung);
    }
}