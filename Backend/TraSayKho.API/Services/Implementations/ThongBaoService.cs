using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class ThongBaoService : IThongBaoService
    {
        private readonly IThongBaoRepository _repository;
        public ThongBaoService(IThongBaoRepository repository) => _repository = repository;

        public async Task<List<ThongBaoDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<List<ThongBaoDto>> GetByKhachHangIdAsync(int khachHangId)
        {
            var list = await _repository.GetByKhachHangIdAsync(khachHangId);
            return list.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage, int SoLuongDaGui)> CreateAsync(ThongBaoCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TieuDe))
                return (false, "Tiêu đề không được để trống.", 0);

            List<int> danhSachKhachHangId;

            if (dto.KhachHangId.HasValue)
            {
                // Gửi cho 1 khách hàng cụ thể
                danhSachKhachHangId = new List<int> { dto.KhachHangId.Value };
            }
            else
            {
                // Gửi cho TẤT CẢ khách hàng
                danhSachKhachHangId = await _repository.GetAllKhachHangIdsAsync();
            }

            if (danhSachKhachHangId.Count == 0)
                return (false, "Không tìm thấy khách hàng nào để gửi.", 0);

            var danhSachThongBao = danhSachKhachHangId.Select(khachHangId => new ThongBao
            {
                KhachHangId = khachHangId,
                TieuDe = dto.TieuDe,
                NoiDung = dto.NoiDung,
                DaDoc = false,
                NgayTao = DateTime.Now
            }).ToList();

            await _repository.AddRangeAsync(danhSachThongBao);
            return (true, null, danhSachThongBao.Count);
        }

        private static ThongBaoDto MapToDto(ThongBao tb) => new()
        {
            ThongBaoId = tb.ThongBaoId,
            KhachHangId = tb.KhachHangId,
            TenKhachHang = tb.KhachHang.HoTen,
            TieuDe = tb.TieuDe,
            NoiDung = tb.NoiDung,
            DaDoc = tb.DaDoc,
            NgayTao = tb.NgayTao
        };
    }
}