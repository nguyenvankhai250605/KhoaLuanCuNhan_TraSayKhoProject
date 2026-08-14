using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IKhachHangService
    {
        Task<List<KhachHangDto>> GetAllAsync();
        Task<KhachHangDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, KhachHangUpdateDto dto);
        Task<bool> SetTrangThaiTaiKhoanAsync(int id, bool trangThai);
    }
}