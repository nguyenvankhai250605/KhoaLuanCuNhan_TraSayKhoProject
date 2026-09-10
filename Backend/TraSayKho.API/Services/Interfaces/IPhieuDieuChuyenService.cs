using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IPhieuDieuChuyenService
    {
        Task<List<PhieuDieuChuyenDto>> GetAllAsync();
        Task<PhieuDieuChuyenDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage, PhieuDieuChuyenDto? Result)> TaoYeuCauAsync(PhieuDieuChuyenCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> DuyetAsync(int id);
        Task<(bool Success, string? ErrorMessage)> TuChoiAsync(int id, TuChoiPhieuDto dto);
        Task<(bool Success, string? ErrorMessage)> XacNhanXuatKhoAsync(int id, XacNhanThucHienDto dto);
        Task<(bool Success, string? ErrorMessage)> XacNhanNhanHangAsync(int id, XacNhanThucHienDto dto);
    }
}