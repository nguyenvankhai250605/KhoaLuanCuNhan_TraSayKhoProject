using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IGioHangService
    {
        Task<(bool Success, string? ErrorMessage, GioHangDto? Result)> GetGioHangAsync(int khachHangId);
        Task<(bool Success, string? ErrorMessage)> ThemVaoGioHangAsync(int khachHangId, ThemVaoGioHangDto dto);
        Task<(bool Success, string? ErrorMessage)> CapNhatSoLuongAsync(int chiTietId, CapNhatSoLuongGioHangDto dto);
        Task<(bool Success, string? ErrorMessage)> XoaChiTietAsync(int chiTietId);
        Task<(bool Success, string? ErrorMessage)> XoaToanBoAsync(int khachHangId);
    }
}