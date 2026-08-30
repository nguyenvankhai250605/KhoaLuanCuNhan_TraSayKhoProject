using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface ILoHangService
    {
        Task<List<LoHangDto>> GetAllAsync();
        Task<LoHangDto?> GetByIdAsync(int id);
        Task<List<LoHangDto>> GetBySanPhamAsync(int sanPhamId);
        Task<List<LoHangDto>> GetSapHetHanAsync(int soNgayNguong);
        Task<(bool Success, string? ErrorMessage, LoHangDto? Result)> CreateAsync(LoHangCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> BatXaKhoAsync(int loHangId, XaKhoDto dto);
        Task<(bool Success, string? ErrorMessage)> HuyXaKhoAsync(int loHangId);
    }
}