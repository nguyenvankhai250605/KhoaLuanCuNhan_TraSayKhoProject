using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class HinhAnhSanPhamService : IHinhAnhSanPhamService
    {
        private readonly IHinhAnhSanPhamRepository _repository;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] DuoiFileChoPhep = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long KichThuocToiDa = 5 * 1024 * 1024; // 5MB

        public HinhAnhSanPhamService(IHinhAnhSanPhamRepository repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        public async Task<List<HinhAnhSanPhamDto>> GetBySanPhamIdAsync(int sanPhamId)
        {
            var list = await _repository.GetBySanPhamIdAsync(sanPhamId);
            return list.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage, HinhAnhSanPhamDto? Result)> UploadAsync(
            int sanPhamId, IFormFile file, int thuTuHienThi)
        {
            if (!await _repository.SanPhamExistsAsync(sanPhamId))
                return (false, "Sản phẩm không tồn tại.", null);

            if (file == null || file.Length == 0)
                return (false, "Vui lòng chọn file ảnh.", null);

            if (file.Length > KichThuocToiDa)
                return (false, "Kích thước ảnh không được vượt quá 5MB.", null);

            var duoiFile = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!DuoiFileChoPhep.Contains(duoiFile))
                return (false, "Chỉ chấp nhận file ảnh định dạng .jpg, .jpeg, .png, .webp.", null);

            // Tạo tên file duy nhất, tránh trùng lặp/ghi đè
            var tenFileMoi = $"{Guid.NewGuid()}{duoiFile}";
            var thuMucLuu = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "products");

            // Tự tạo thư mục nếu chưa tồn tại (phòng trường hợp bị xóa nhầm)
            Directory.CreateDirectory(thuMucLuu);

            var duongDanDayDu = Path.Combine(thuMucLuu, tenFileMoi);

            using (var fileStream = new FileStream(duongDanDayDu, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var duongDanLuuDb = $"/images/products/{tenFileMoi}";

            var hinhAnh = new HinhAnhSanPham
            {
                SanPhamId = sanPhamId,
                DuongDanAnh = duongDanLuuDb,
                ThuTuHienThi = thuTuHienThi
            };

            var created = await _repository.AddAsync(hinhAnh);
            return (true, null, MapToDto(created));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hinhAnh = await _repository.GetByIdAsync(id);
            if (hinhAnh == null) return false;

            // Xóa file vật lý trên server trước khi xóa record trong DB
            var duongDanVatLy = Path.Combine(_environment.ContentRootPath, "wwwroot",
                hinhAnh.DuongDanAnh.TrimStart('/').Replace("images/", "images" + Path.DirectorySeparatorChar));

            if (File.Exists(duongDanVatLy))
            {
                File.Delete(duongDanVatLy);
            }

            return await _repository.DeleteAsync(id);
        }

        private static HinhAnhSanPhamDto MapToDto(HinhAnhSanPham ha) => new()
        {
            HinhAnhId = ha.HinhAnhId,
            DuongDanAnh = ha.DuongDanAnh,
            ThuTuHienThi = ha.ThuTuHienThi
        };
    }
}