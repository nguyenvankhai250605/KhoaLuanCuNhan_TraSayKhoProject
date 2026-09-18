using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly IDanhGiaRepository _repository;
        public DanhGiaService(IDanhGiaRepository repository) => _repository = repository;

        public async Task<List<DanhGiaDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<DanhGiaDto?> GetByIdAsync(int id)
        {
            var dg = await _repository.GetByIdAsync(id);
            return dg == null ? null : MapToDto(dg);
        }

        public async Task<(bool Success, string? ErrorMessage, DanhGiaDto? Result)> CreateAsync(
            int khachHangId, DanhGiaCreateDto dto)
        {
            if (dto.SoSao < 1 || dto.SoSao > 5)
                return (false, "Số sao phải từ 1 đến 5.", null);

            if (dto.NoiDung?.Length > 500)
                return (false, "Nội dung đánh giá không được vượt quá 500 ký tự.", null);

            var donHang = await _repository.GetDonHangAsync(dto.DonHangId);
            if (donHang == null || donHang.KhachHangId != khachHangId)
                return (false, "Đơn hàng không tồn tại hoặc không thuộc tài khoản của bạn.", null);

            if (donHang.TrangThai.TenTrangThai != "HoanThanh")
                return (false, "Chỉ được đánh giá đơn hàng đã hoàn thành.", null);

            if (!donHang.ChiTietDonHangs.Any(ct => ct.SanPhamId == dto.SanPhamId))
                return (false, "Sản phẩm này không có trong đơn hàng.", null);

            if (await _repository.ExistsAsync(dto.DonHangId, dto.SanPhamId))
                return (false, "Bạn đã đánh giá sản phẩm này trong đơn hàng.", null);

            var danhGia = await _repository.CreateAsync(new DanhGium
            {
                DonHangId = dto.DonHangId,
                SanPhamId = dto.SanPhamId,
                KhachHangId = khachHangId,
                SoSao = dto.SoSao,
                NoiDung = string.IsNullOrWhiteSpace(dto.NoiDung) ? null : dto.NoiDung.Trim(),
                NgayDanhGia = DateTime.Now
            });

            return (true, null, MapToDto(danhGia));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static DanhGiaDto MapToDto(DanhGium dg) => new()
        {
            DanhGiaId = dg.DanhGiaId,
            TenSanPham = dg.SanPham.TenSanPham,
            TenKhachHang = dg.KhachHang.HoTen,
            SoSao = dg.SoSao,
            NoiDung = dg.NoiDung,
            NgayDanhGia = dg.NgayDanhGia
        };
    }
}
