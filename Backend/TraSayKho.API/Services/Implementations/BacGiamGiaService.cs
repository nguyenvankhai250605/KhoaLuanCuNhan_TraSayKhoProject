using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class BacGiamGiaService : IBacGiamGiaService
    {
        private readonly IBacGiamGiaRepository _repository;
        public BacGiamGiaService(IBacGiamGiaRepository repository) => _repository = repository;

        public async Task<List<BacGiamGiaDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage, BacGiamGiaDto? Result)> CreateAsync(BacGiamGiaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenBac))
                return (false, "Tên bậc không được để trống.", null);

            if (dto.PhanTramThoiGianConLaiToiDa <= 0 || dto.PhanTramThoiGianConLaiToiDa > 100)
                return (false, "Phần trăm thời gian còn lại phải trong khoảng 0-100.", null);

            if (dto.MucGiamGiaPhanTram <= 0 || dto.MucGiamGiaPhanTram > 100)
                return (false, "Mức giảm giá phải trong khoảng 0-100%.", null);

            if (dto.DanhMucId.HasValue && !await _repository.DanhMucExistsAsync(dto.DanhMucId.Value))
                return (false, "Danh mục không tồn tại.", null);

            var bac = new BacGiamGiaXaKho
            {
                DanhMucId = dto.DanhMucId,
                TenBac = dto.TenBac,
                PhanTramThoiGianConLaiToiDa = dto.PhanTramThoiGianConLaiToiDa,
                MucGiamGiaPhanTram = dto.MucGiamGiaPhanTram,
                TrangThai = true
            };

            var created = await _repository.AddAsync(bac);
            var full = await _repository.GetByIdAsync(created.BacGiamGiaId);
            return (true, null, MapToDto(full!));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, BacGiamGiaUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenBac))
                return (false, "Tên bậc không được để trống.");

            if (dto.PhanTramThoiGianConLaiToiDa <= 0 || dto.PhanTramThoiGianConLaiToiDa > 100)
                return (false, "Phần trăm thời gian còn lại phải trong khoảng 0-100.");

            if (dto.MucGiamGiaPhanTram <= 0 || dto.MucGiamGiaPhanTram > 100)
                return (false, "Mức giảm giá phải trong khoảng 0-100%.");

            if (dto.DanhMucId.HasValue && !await _repository.DanhMucExistsAsync(dto.DanhMucId.Value))
                return (false, "Danh mục không tồn tại.");

            var bac = new BacGiamGiaXaKho
            {
                BacGiamGiaId = id,
                DanhMucId = dto.DanhMucId,
                TenBac = dto.TenBac,
                PhanTramThoiGianConLaiToiDa = dto.PhanTramThoiGianConLaiToiDa,
                MucGiamGiaPhanTram = dto.MucGiamGiaPhanTram,
                TrangThai = dto.TrangThai
            };

            var success = await _repository.UpdateAsync(id, bac);
            return success ? (true, null) : (false, "Không tìm thấy bậc giảm giá.");
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static BacGiamGiaDto MapToDto(BacGiamGiaXaKho b) => new()
        {
            BacGiamGiaId = b.BacGiamGiaId,
            DanhMucId = b.DanhMucId,
            TenDanhMuc = b.DanhMuc?.TenDanhMuc,
            TenBac = b.TenBac,
            PhanTramThoiGianConLaiToiDa = b.PhanTramThoiGianConLaiToiDa,
            MucGiamGiaPhanTram = b.MucGiamGiaPhanTram,
            TrangThai = b.TrangThai
        };
    }
}