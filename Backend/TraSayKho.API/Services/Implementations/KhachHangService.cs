using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepository _repository;
        public KhachHangService(IKhachHangRepository repository) => _repository = repository;

        public async Task<List<KhachHangDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<KhachHangDto?> GetByIdAsync(int id)
        {
            var kh = await _repository.GetByIdAsync(id);
            return kh == null ? null : MapToDto(kh);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, KhachHangUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.HoTen))
                return (false, "Họ tên không được để trống.");

            var thongTinMoi = new KhachHang
            {
                HoTen = dto.HoTen,
                DiaChi = dto.DiaChi,
                NgaySinh = dto.NgaySinh,
                GioiTinh = dto.GioiTinh
            };

            var success = await _repository.UpdateAsync(id, thongTinMoi);
            return success ? (true, null) : (false, "Không tìm thấy khách hàng.");
        }

        public async Task<bool> SetTrangThaiTaiKhoanAsync(int id, bool trangThai)
        {
            return await _repository.SetTrangThaiTaiKhoanAsync(id, trangThai);
        }

        private static KhachHangDto MapToDto(KhachHang kh) => new()
        {
            KhachHangId = kh.KhachHangId,
            HoTen = kh.HoTen,
            DiaChi = kh.DiaChi,
            Email = kh.TaiKhoan.Email,
            SoDienThoai = kh.TaiKhoan.SoDienThoai,
            TrangThaiTaiKhoan = kh.TaiKhoan.TrangThai
        };
    }
}