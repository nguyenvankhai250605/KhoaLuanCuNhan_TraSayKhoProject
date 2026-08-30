using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class ChiNhanhService : IChiNhanhService
    {
        private readonly IChiNhanhRepository _repository;
        public ChiNhanhService(IChiNhanhRepository repository) => _repository = repository;

        public async Task<List<ChiNhanhDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<ChiNhanhDto?> GetByIdAsync(int id)
        {
            var cn = await _repository.GetByIdAsync(id);
            return cn == null ? null : MapToDto(cn);
        }

        public async Task<(bool Success, string? ErrorMessage, ChiNhanhDto? Result)> CreateAsync(ChiNhanhCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenChiNhanh))
                return (false, "Tên chi nhánh không được để trống.", null);

            var chiNhanh = new ChiNhanh
            {
                TenChiNhanh = dto.TenChiNhanh,
                DiaChi = dto.DiaChi,
                SoDienThoai = dto.SoDienThoai,
                LaTruSoChinh = false,
                TrangThai = true
            };

            var created = await _repository.AddAsync(chiNhanh);
            return (true, null, MapToDto(created));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, ChiNhanhUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenChiNhanh))
                return (false, "Tên chi nhánh không được để trống.");

            var chiNhanh = new ChiNhanh
            {
                ChiNhanhId = id,
                TenChiNhanh = dto.TenChiNhanh,
                DiaChi = dto.DiaChi,
                SoDienThoai = dto.SoDienThoai,
                TrangThai = dto.TrangThai
            };

            var success = await _repository.UpdateAsync(id, chiNhanh);
            return success ? (true, null) : (false, "Không tìm thấy chi nhánh.");
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static ChiNhanhDto MapToDto(ChiNhanh cn) => new()
        {
            ChiNhanhId = cn.ChiNhanhId,
            TenChiNhanh = cn.TenChiNhanh,
            DiaChi = cn.DiaChi,
            SoDienThoai = cn.SoDienThoai,
            LaTruSoChinh = cn.LaTruSoChinh,
            TrangThai = cn.TrangThai
        };
    }
}