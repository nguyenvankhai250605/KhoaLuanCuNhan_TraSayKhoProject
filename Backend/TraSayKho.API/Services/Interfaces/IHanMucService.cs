using TraSayKho.API.DTOs;

namespace TraSayKho.API.Services.Interfaces
{
    public interface IHanMucService
    {
        Task<List<HanMucDto>> GetAllAsync();
        Task<(bool Success, string? ErrorMessage, HanMucDto? Result)> CreateAsync(HanMucCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, HanMucUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<List<CanhBaoTonKhoDto>> GetCanhBaoTonKhoAsync();

        // Dùng nội bộ khi tạo đơn hàng — không lộ ra Controller
        Task<(bool HopLe, string? LyDoTuChoi)> KiemTraHanMucBanAsync(int sanPhamId, int? chiNhanhId, int soLuongDatMua);
    }
}