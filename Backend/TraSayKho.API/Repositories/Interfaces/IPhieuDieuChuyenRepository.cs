using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IPhieuDieuChuyenRepository
    {
        Task<bool> ChiNhanhExistsAsync(int chiNhanhId);

        Task<bool> NhanVienExistsAsync(int nhanVienId);

        // Lấy các thùng của sản phẩm tại một chi nhánh
        // và sắp xếp theo FEFO dựa vào hạn sử dụng của lô
        Task<List<ThungHang>> GetThungHangTheoFefoAsync(
            int sanPhamId,
            int chiNhanhId);

        Task<List<PhieuDieuChuyenKho>> GetAllAsync();

        Task<PhieuDieuChuyenKho?> GetByIdAsync(int id);

        Task<PhieuDieuChuyenKho> TaoYeuCauAsync(
            PhieuDieuChuyenKho phieu,
            List<ChiTietPhieuDieuChuyen> chiTiets);

        Task<bool> DuyetAsync(int phieuId);

        Task<bool> TuChoiAsync(
            int phieuId,
            string lyDo);

        Task<bool> XacNhanXuatKhoAsync(
            int phieuId,
            int nhanVienId);

        Task<bool> XacNhanNhanHangAsync(
            int phieuId,
            int nhanVienId);

        Task DongBoTonKhoSanPhamAsync(int sanPhamId);
    }
}