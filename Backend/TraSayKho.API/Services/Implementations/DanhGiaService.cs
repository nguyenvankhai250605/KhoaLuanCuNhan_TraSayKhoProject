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