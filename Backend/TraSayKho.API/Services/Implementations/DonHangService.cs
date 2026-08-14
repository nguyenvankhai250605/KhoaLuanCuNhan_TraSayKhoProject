using TraSayKho.API.DTOs;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _repository;

        // Định nghĩa các bước hợp lệ — không cho nhảy cóc trạng thái tùy tiện
        private static readonly Dictionary<string, string[]> QuyTacChuyenTrangThai = new()
        {
            ["ChoXacNhan"] = new[] { "DangXuLy", "DaHuy" },
            ["DangXuLy"] = new[] { "DangGiao", "DaHuy" },
            ["DangGiao"] = new[] { "DaGiao", "DaHuy" },
            ["DaGiao"] = new[] { "HoanThanh" },
            ["HoanThanh"] = Array.Empty<string>(),
            ["DaHuy"] = Array.Empty<string>()
        };

        public DonHangService(IDonHangRepository repository) => _repository = repository;

        public async Task<List<DonHangDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(dh => new DonHangDto
            {
                DonHangId = dh.DonHangId,
                TenKhachHang = dh.KhachHang.HoTen,
                TrangThai = dh.TrangThai.TenTrangThai,
                TongTien = dh.TongTien,
                NgayDatHang = dh.NgayDatHang
            }).ToList();
        }

        public async Task<DonHangChiTietDto?> GetByIdAsync(int id)
        {
            var dh = await _repository.GetByIdWithDetailsAsync(id);
            if (dh == null) return null;

            return new DonHangChiTietDto
            {
                DonHangId = dh.DonHangId,
                TenKhachHang = dh.KhachHang.HoTen,
                TrangThai = dh.TrangThai.TenTrangThai,
                DiaChiGiaoHang = dh.DiaChiGiaoHang,
                TongTien = dh.TongTien,
                NgayDatHang = dh.NgayDatHang,
                ChiTietSanPhams = dh.ChiTietDonHangs.Select(ct => new ChiTietSanPhamTrongDonDto
                {
                    TenSanPham = ct.SanPham.TenSanPham,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien ?? 0
                }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatTrangThaiAsync(int id, CapNhatTrangThaiDto dto)
        {
            var donHangHienTai = await _repository.GetByIdAsync(id);
            if (donHangHienTai == null)
                return (false, "Không tìm thấy đơn hàng.");

            var trangThaiHienTai = await _repository.GetTrangThaiByTenAsync(
                (await _repository.GetByIdWithDetailsAsync(id))!.TrangThai.TenTrangThai);

            var trangThaiMoi = await _repository.GetTrangThaiByTenAsync(dto.TenTrangThaiMoi);
            if (trangThaiMoi == null)
                return (false, "Trạng thái không hợp lệ.");

            // Kiểm tra quy tắc chuyển trạng thái hợp lệ
            var tenTrangThaiHienTai = trangThaiHienTai!.TenTrangThai;
            if (!QuyTacChuyenTrangThai.TryGetValue(tenTrangThaiHienTai, out var cacBuocDuocPhep) ||
                !cacBuocDuocPhep.Contains(dto.TenTrangThaiMoi))
            {
                return (false, $"Không thể chuyển từ trạng thái '{tenTrangThaiHienTai}' sang '{dto.TenTrangThaiMoi}'.");
            }

            var success = await _repository.UpdateTrangThaiAsync(id, trangThaiMoi.TrangThaiId);
            return success ? (true, null) : (false, "Cập nhật thất bại.");
        }
    }
}