using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IHinhAnhSanPhamService
    {
        Task<List<HinhAnhSanPhamDto>> GetBySanPhamIdAsync(int sanPhamId);
        Task<(bool Success, string? ErrorMessage, HinhAnhSanPhamDto? Result)> UploadAsync(int sanPhamId, IFormFile file, int thuTuHienThi);
        Task<bool> DeleteAsync(int id);
    }
}