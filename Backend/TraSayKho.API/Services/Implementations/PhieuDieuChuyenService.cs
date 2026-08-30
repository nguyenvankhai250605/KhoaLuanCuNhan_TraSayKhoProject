using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class PhieuDieuChuyenService : IPhieuDieuChuyenService
    {
        private readonly IPhieuDieuChuyenRepository _repository;
        public PhieuDieuChuyenService(IPhieuDieuChuyenRepository repository) => _repository = repository;

        public async Task<List<PhieuDieuChuyenDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<PhieuDieuChuyenDto?> GetByIdAsync(int id)
        {
            var phieu = await _repository.GetByIdAsync(id);
            return phieu == null ? null : MapToDto(phieu);
        }

        public async Task<(bool Success, string? ErrorMessage, PhieuDieuChuyenDto? Result)> CreateAsync(PhieuDieuChuyenCreateDto dto)
        {
            if (dto.ChiNhanhGuiId == dto.ChiNhanhNhanId)
                return (false, "Chi nhánh gửi và chi nhánh nhận phải khác nhau.", null);

            if (!await _repository.ChiNhanhExistsAsync(dto.ChiNhanhGuiId))
                return (false, "Chi nhánh gửi không tồn tại.", null);

            if (!await _repository.ChiNhanhExistsAsync(dto.ChiNhanhNhanId))
                return (false, "Chi nhánh nhận không tồn tại.", null);

            if (!await _repository.NhanVienExistsAsync(dto.NhanVienTaoId))
                return (false, "Nhân viên tạo phiếu không tồn tại.", null);

            if (dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                return (false, "Phiếu điều chuyển phải có ít nhất 1 dòng sản phẩm.", null);

            // Kiểm tra từng lô: phải thuộc đúng chi nhánh gửi, và đủ số lượng
            foreach (var ct in dto.ChiTiet)
            {
                var loHang = await _repository.GetLoHangByIdAsync(ct.LoHangId);

                if (loHang == null)
                    return (false, $"Không tìm thấy lô hàng ID {ct.LoHangId}.", null);

                if (loHang.ChiNhanhId != dto.ChiNhanhGuiId)
                    return (false, $"Lô hàng '{loHang.SoLo}' không thuộc chi nhánh gửi.", null);

                if (ct.SoLuong <= 0)
                    return (false, "Số lượng điều chuyển phải lớn hơn 0.", null);

                if (ct.SoLuong > loHang.SoLuongConLai)
                    return (false, $"Lô hàng '{loHang.SoLo}' chỉ còn {loHang.SoLuongConLai}, không đủ để chuyển {ct.SoLuong}.", null);
            }

            var phieu = new PhieuDieuChuyenKho
            {
                ChiNhanhGuiId = dto.ChiNhanhGuiId,
                ChiNhanhNhanId = dto.ChiNhanhNhanId,
                NhanVienTaoId = dto.NhanVienTaoId,
                GhiChu = dto.GhiChu,
                TrangThai = "ChoXacNhan",
                NgayTao = DateTime.Now
            };

            var chiTiets = dto.ChiTiet.Select(ct => new ChiTietPhieuDieuChuyen
            {
                LoHangId = ct.LoHangId,
                SoLuong = ct.SoLuong
            }).ToList();

            var created = await _repository.CreateAsync(phieu, chiTiets);
            return (true, null, MapToDto(created));
        }

        public async Task<(bool Success, string? ErrorMessage)> XacNhanAsync(int id, XacNhanPhieuDto dto)
        {
            var phieu = await _repository.GetByIdAsync(id);
            if (phieu == null)
                return (false, "Không tìm thấy phiếu điều chuyển.");

            if (phieu.TrangThai != "ChoXacNhan")
                return (false, "Phiếu này đã được xử lý trước đó (đã xác nhận hoặc đã hủy).");

            if (!await _repository.NhanVienExistsAsync(dto.NhanVienXacNhanId))
                return (false, "Nhân viên xác nhận không tồn tại.");

            var success = await _repository.XacNhanAsync(id, dto.NhanVienXacNhanId);
            return success ? (true, null) : (false, "Không thể xác nhận phiếu điều chuyển.");
        }

        public async Task<(bool Success, string? ErrorMessage)> HuyAsync(int id)
        {
            var phieu = await _repository.GetByIdAsync(id);
            if (phieu == null)
                return (false, "Không tìm thấy phiếu điều chuyển.");

            if (phieu.TrangThai != "ChoXacNhan")
                return (false, "Chỉ có thể hủy phiếu đang ở trạng thái chờ xác nhận.");

            var success = await _repository.HuyAsync(id);
            return success ? (true, null) : (false, "Không thể hủy phiếu điều chuyển.");
        }

        private static PhieuDieuChuyenDto MapToDto(PhieuDieuChuyenKho p) => new()
        {
            PhieuDieuChuyenId = p.PhieuDieuChuyenId,
            ChiNhanhGuiId = p.ChiNhanhGuiId,
            TenChiNhanhGui = p.ChiNhanhGui.TenChiNhanh,
            ChiNhanhNhanId = p.ChiNhanhNhanId,
            TenChiNhanhNhan = p.ChiNhanhNhan.TenChiNhanh,
            TenNhanVienTao = p.NhanVienTao.HoTen,
            TenNhanVienXacNhan = p.NhanVienXacNhan?.HoTen,
            TrangThai = p.TrangThai,
            GhiChu = p.GhiChu,
            NgayTao = p.NgayTao,
            NgayXacNhan = p.NgayXacNhan,
            ChiTiet = p.ChiTietPhieuDieuChuyens.Select(ct => new ChiTietPhieuDieuChuyenDto
            {
                ChiTietId = ct.ChiTietId,
                LoHangId = ct.LoHangId,
                SoLo = ct.LoHang.SoLo,
                TenSanPham = ct.LoHang.SanPham.TenSanPham,
                HanSuDung = ct.LoHang.HanSuDung,
                SoLuong = ct.SoLuong
            }).ToList()
        };
    }
}