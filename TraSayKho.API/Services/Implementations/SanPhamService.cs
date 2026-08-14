using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _repository;
        public SanPhamService(ISanPhamRepository repository) => _repository = repository;

        public async Task<List<SanPhamDto>> GetAllAsync()
        {
            var sanPhams = await _repository.GetAllAsync();
            return sanPhams.Select(MapToDto).ToList();
        }

        public async Task<SanPhamDto?> GetByIdAsync(int id)
        {
            var sp = await _repository.GetByIdAsync(id);
            return sp == null ? null : MapToDto(sp);
        }

        public async Task<(bool Success, string? ErrorMessage, SanPhamDto? Result)> CreateAsync(SanPhamCreateDto dto)
        {
            // Kiểm tra nghiệp vụ trước khi lưu
            if (dto.GiaBan < 0)
                return (false, "Giá bán không được nhỏ hơn 0.", null);

            if (dto.SoLuongTon < 0)
                return (false, "Số lượng tồn không được nhỏ hơn 0.", null);

            if (!await _repository.DanhMucExistsAsync(dto.DanhMucId))
                return (false, "Danh mục không tồn tại.", null);

            var sanPham = new SanPham
            {
                TenSanPham = dto.TenSanPham,
                DanhMucId = dto.DanhMucId,
                MoTaChiTiet = dto.MoTaChiTiet,
                XuatXu = dto.XuatXu,
                GiaBan = dto.GiaBan,
                SoLuongTon = dto.SoLuongTon,
                DonViTinh = dto.DonViTinh,
                HanSuDung = dto.HanSuDung,
                TrangThai = "DangBan"
            };

            var created = await _repository.AddAsync(sanPham);
            return (true, null, MapToDto(created));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, SanPhamUpdateDto dto)
        {
            if (dto.GiaBan < 0)
                return (false, "Giá bán không được nhỏ hơn 0.");

            if (dto.SoLuongTon < 0)
                return (false, "Số lượng tồn không được nhỏ hơn 0.");

            if (!await _repository.DanhMucExistsAsync(dto.DanhMucId))
                return (false, "Danh mục không tồn tại.");

            var sanPham = new SanPham
            {
                SanPhamId = id,
                TenSanPham = dto.TenSanPham,
                DanhMucId = dto.DanhMucId,
                MoTaChiTiet = dto.MoTaChiTiet,
                XuatXu = dto.XuatXu,
                GiaBan = dto.GiaBan,
                SoLuongTon = dto.SoLuongTon,
                DonViTinh = dto.DonViTinh,
                HanSuDung = dto.HanSuDung,
                TrangThai = dto.TrangThai
            };

            var success = await _repository.UpdateAsync(sanPham);
            return success ? (true, null) : (false, "Không tìm thấy sản phẩm.");
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static SanPhamDto MapToDto(SanPham sp) => new()
        {
            SanPhamId = sp.SanPhamId,
            TenSanPham = sp.TenSanPham,
            TenDanhMuc = sp.DanhMuc.TenDanhMuc,
            GiaBan = sp.GiaBan,
            SoLuongTon = sp.SoLuongTon,
            TrangThai = sp.TrangThai
        };
    }
}