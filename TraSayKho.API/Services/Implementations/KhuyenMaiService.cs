using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class KhuyenMaiService : IKhuyenMaiService
    {
        private readonly IKhuyenMaiRepository _repository;
        public KhuyenMaiService(IKhuyenMaiRepository repository) => _repository = repository;

        public async Task<List<KhuyenMaiDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<KhuyenMaiDto?> GetByIdAsync(int id)
        {
            var km = await _repository.GetByIdAsync(id);
            return km == null ? null : MapToDto(km);
        }

        public async Task<(bool Success, string? ErrorMessage, KhuyenMaiDto? Result)> CreateAsync(KhuyenMaiCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaCode))
                return (false, "Mã code không được để trống.", null);

            if (await _repository.MaCodeExistsAsync(dto.MaCode))
                return (false, "Mã code đã tồn tại, vui lòng chọn mã khác.", null);

            if (dto.NgayKetThuc < dto.NgayBatDau)
                return (false, "Ngày kết thúc phải sau ngày bắt đầu.", null);

            if (dto.GiaTriGiam <= 0)
                return (false, "Giá trị giảm phải lớn hơn 0.", null);

            if (dto.LoaiGiam == "PhanTram" && dto.GiaTriGiam > 100)
                return (false, "Giảm theo phần trăm không được vượt quá 100%.", null);

            var khuyenMai = new KhuyenMai
            {
                MaCode = dto.MaCode,
                MoTa = dto.MoTa,
                LoaiGiam = dto.LoaiGiam,
                GiaTriGiam = dto.GiaTriGiam,
                GiaTriDonHangToiThieu = dto.GiaTriDonHangToiThieu,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                SoLuotSuDungToiDa = dto.SoLuotSuDungToiDa,
                SoLuotDaSuDung = 0,
                TrangThai = true
            };

            var created = await _repository.AddAsync(khuyenMai);
            return (true, null, MapToDto(created));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, KhuyenMaiUpdateDto dto)
        {
            if (dto.NgayKetThuc < dto.NgayBatDau)
                return (false, "Ngày kết thúc phải sau ngày bắt đầu.");

            if (dto.GiaTriGiam <= 0)
                return (false, "Giá trị giảm phải lớn hơn 0.");

            if (dto.LoaiGiam == "PhanTram" && dto.GiaTriGiam > 100)
                return (false, "Giảm theo phần trăm không được vượt quá 100%.");

            var khuyenMai = new KhuyenMai
            {
                KhuyenMaiId = id,
                MoTa = dto.MoTa,
                LoaiGiam = dto.LoaiGiam,
                GiaTriGiam = dto.GiaTriGiam,
                GiaTriDonHangToiThieu = dto.GiaTriDonHangToiThieu,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                SoLuotSuDungToiDa = dto.SoLuotSuDungToiDa,
                TrangThai = dto.TrangThai
            };

            var success = await _repository.UpdateAsync(khuyenMai);
            return success ? (true, null) : (false, "Không tìm thấy khuyến mãi.");
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static KhuyenMaiDto MapToDto(KhuyenMai km) => new()
        {
            KhuyenMaiId = km.KhuyenMaiId,
            MaCode = km.MaCode,
            MoTa = km.MoTa,
            GiaTriGiam = km.GiaTriGiam,
            NgayBatDau = km.NgayBatDau,
            NgayKetThuc = km.NgayKetThuc,
            TrangThai = km.TrangThai
        };
    }
}