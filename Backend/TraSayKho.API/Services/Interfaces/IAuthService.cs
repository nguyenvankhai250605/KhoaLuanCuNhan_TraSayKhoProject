using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> DangKyKhachHangAsync(DangKyDto dto);
        Task<(bool Success, string? ErrorMessage)> TaoNhanVienAsync(TaoTaiKhoanNhanVienDto dto);
        Task<(bool Success, string? ErrorMessage, DangNhapResponseDto? Result)> DangNhapAsync(DangNhapDto dto);
    }
}