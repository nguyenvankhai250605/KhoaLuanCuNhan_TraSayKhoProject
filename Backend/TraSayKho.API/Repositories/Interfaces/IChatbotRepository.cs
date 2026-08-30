using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface IChatbotRepository
    {
        Task<bool> KhachHangExistsAsync(int khachHangId);
        Task<CuocHoiThoai?> GetCuocHoiThoaiDangMoAsync(int khachHangId);
        Task<CuocHoiThoai> TaoCuocHoiThoaiMoiAsync(int khachHangId);
        Task<TinNhan?> GetTinNhanGanNhatAsync(int cuocHoiThoaiId);
        Task<bool> DongPhienAsync(int cuocHoiThoaiId);
        Task<CuocHoiThoai?> GetCuocHoiThoaiByIdAsync(int cuocHoiThoaiId);
        Task<TinNhan> AddTinNhanAsync(int cuocHoiThoaiId, string nguoiGui, string noiDung);
        Task<List<TinNhan>> GetLichSuGanNhatAsync(int cuocHoiThoaiId, int soLuong);
        Task<List<TinNhan>> GetToanBoLichSuAsync(int cuocHoiThoaiId);
        Task<List<SanPham>> GetSanPhamDangBanKemChiTietAsync();
    }
}