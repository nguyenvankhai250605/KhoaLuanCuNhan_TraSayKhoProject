using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class DanhMucService : IDanhMucService
    {
        private readonly IDanhMucRepository _repository;
        public DanhMucService(IDanhMucRepository repository) => _repository = repository;

        public async Task<List<DanhMucDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<DanhMucDto?> GetByIdAsync(int id)
        {
            var dm = await _repository.GetByIdAsync(id);
            return dm == null ? null : MapToDto(dm);
        }

        public async Task<(bool Success, string? ErrorMessage, DanhMucDto? Result)> CreateAsync(DanhMucCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenDanhMuc))
                return (false, "Tên danh mục không được để trống.", null);

            var danhMuc = new DanhMuc
            {
                TenDanhMuc = dto.TenDanhMuc,
                MoTa = dto.MoTa,
                TrangThai = true
            };

            var created = await _repository.AddAsync(danhMuc);
            return (true, null, MapToDto(created));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, DanhMucUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenDanhMuc))
                return (false, "Tên danh mục không được để trống.");

            var danhMuc = new DanhMuc
            {
                DanhMucId = id,
                TenDanhMuc = dto.TenDanhMuc,
                MoTa = dto.MoTa,
                TrangThai = dto.TrangThai
            };

            var success = await _repository.UpdateAsync(danhMuc);
            return success ? (true, null) : (false, "Không tìm thấy danh mục.");
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static DanhMucDto MapToDto(DanhMuc dm) => new()
        {
            DanhMucId = dm.DanhMucId,
            TenDanhMuc = dm.TenDanhMuc,
            MoTa = dm.MoTa,
            TrangThai = dm.TrangThai
        };
    }
}