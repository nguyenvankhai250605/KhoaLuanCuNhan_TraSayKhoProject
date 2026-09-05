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

            if (dto.SoNgayConLaiToiDa <= 0)
                return (false, "Số ngày còn lại tối đa phải lớn hơn 0.", null);

            if (dto.MucGiamGiaPhanTram <= 0 || dto.MucGiamGiaPhanTram > 100)
                return (false, "Mức giảm giá phải trong khoảng 0-100%.", null);

            var bac = new BacGiamGiaXaKho
            {
                TenBac = dto.TenBac,
                SoNgayConLaiToiDa = dto.SoNgayConLaiToiDa,
                MucGiamGiaPhanTram = dto.MucGiamGiaPhanTram,
                TrangThai = true
            };

            var created = await _repository.AddAsync(bac);
            return (true, null, MapToDto(created));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, BacGiamGiaUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenBac))
                return (false, "Tên bậc không được để trống.");

            if (dto.SoNgayConLaiToiDa <= 0)
                return (false, "Số ngày còn lại tối đa phải lớn hơn 0.");

            if (dto.MucGiamGiaPhanTram <= 0 || dto.MucGiamGiaPhanTram > 100)
                return (false, "Mức giảm giá phải trong khoảng 0-100%.");

            var bac = new BacGiamGiaXaKho
            {
                BacGiamGiaId = id,
                TenBac = dto.TenBac,
                SoNgayConLaiToiDa = dto.SoNgayConLaiToiDa,
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
            TenBac = b.TenBac,
            SoNgayConLaiToiDa = b.SoNgayConLaiToiDa,
            MucGiamGiaPhanTram = b.MucGiamGiaPhanTram,
            TrangThai = b.TrangThai
        };
    }
}