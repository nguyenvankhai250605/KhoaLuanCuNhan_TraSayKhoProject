using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface ILoHangService
    {
        Task<List<LoHangDto>> GetAllAsync();

        Task<LoHangDto?> GetByIdAsync(int id);

        Task<List<LoHangDto>>
            GetBySanPhamAsync(int sanPhamId);

        Task<List<LoHangDto>>
            GetSapHetHanAsync(
                int phanTramNguong);

        Task<(
            bool Success,
            string? ErrorMessage,
            LoHangDto? Result)>
            CreateAsync(
                LoHangCreateDto dto,
                int? chiNhanhNguoiGoi);

        Task<(
            bool Success,
            string? ErrorMessage,
            TruyXuatNguonGocDto? Result)>
            TruyXuatTheoMaDonViAsync(
                string maDonVi);
    }
}