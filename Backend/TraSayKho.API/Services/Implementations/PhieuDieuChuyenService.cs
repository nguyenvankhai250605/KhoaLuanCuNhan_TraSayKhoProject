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

        // BƯỚC 1: Chủ cửa hàng chi nhánh thiếu hàng tạo yêu cầu xin hàng từ chi nhánh khác
        public async Task<(bool Success, string? ErrorMessage, PhieuDieuChuyenDto? Result)> TaoYeuCauAsync(PhieuDieuChuyenCreateDto dto)
        {
            if (dto.ChiNhanhGuiId == dto.ChiNhanhNhanId)
                return (false, "Chi nhánh nguồn và chi nhánh nhận phải khác nhau.", null);

            if (!await _repository.ChiNhanhExistsAsync(dto.ChiNhanhGuiId))
                return (false, "Chi nhánh nguồn (chi nhánh gửi) không tồn tại.", null);

            if (!await _repository.ChiNhanhExistsAsync(dto.ChiNhanhNhanId))
                return (false, "Chi nhánh nhận không tồn tại.", null);

            if (!await _repository.NhanVienExistsAsync(dto.NhanVienTaoId))
                return (false, "Người tạo yêu cầu không tồn tại.", null);

            if (dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                return (false, "Yêu cầu điều chuyển phải có ít nhất 1 sản phẩm.", null);

            var chiTiets = new List<ChiTietPhieuDieuChuyen>();

            // Với mỗi sản phẩm cần xin, tìm đúng lô theo FEFO tại chi nhánh nguồn được chọn
            foreach (var dong in dto.ChiTiet)
            {
                if (dong.SoLuong <= 0)
                    return (false, "Số lượng phải lớn hơn 0.", null);

                var cacLo = await _repository.GetLoHangConHangTheoFefoAsync(dong.SanPhamId, dto.ChiNhanhGuiId);
                var loPhuHop = cacLo.FirstOrDefault(lo => lo.SoLuongConLai >= dong.SoLuong);

                if (loPhuHop == null)
                {
                    var tongConHang = cacLo.Sum(lo => lo.SoLuongConLai);
                    return (false,
                        $"Chi nhánh nguồn không có lô nào đủ {dong.SoLuong} sản phẩm ID {dong.SanPhamId} (tổng tồn hiện có: {tongConHang}).",
                        null);
                }

                chiTiets.Add(new ChiTietPhieuDieuChuyen
                {
                    LoHangId = loPhuHop.LoHangId,
                    SoLuong = dong.SoLuong
                });
            }

            var phieu = new PhieuDieuChuyenKho
            {
                ChiNhanhGuiId = dto.ChiNhanhGuiId,
                ChiNhanhNhanId = dto.ChiNhanhNhanId,
                NhanVienTaoId = dto.NhanVienTaoId,
                GhiChu = dto.GhiChu,
                TrangThai = "ChoDuyet",
                NgayTao = DateTime.Now
            };

            var created = await _repository.TaoYeuCauAsync(phieu, chiTiets);
            return (true, null, MapToDto(created));
        }

        // BƯỚC 2a: Chủ cửa hàng chi nhánh NGUỒN duyệt yêu cầu
        public async Task<(bool Success, string? ErrorMessage)> DuyetAsync(int id)
        {
            var phieu = await _repository.GetByIdAsync(id);
            if (phieu == null) return (false, "Không tìm thấy phiếu điều chuyển.");

            if (phieu.TrangThai != "ChoDuyet")
                return (false, $"Chỉ có thể duyệt phiếu đang ở trạng thái 'Chờ duyệt'. Phiếu này đang ở trạng thái '{phieu.TrangThai}'.");

            var success = await _repository.DuyetAsync(id);
            return success ? (true, null) : (false, "Không thể duyệt phiếu.");
        }

        // BƯỚC 2b: Chủ cửa hàng chi nhánh NGUỒN từ chối yêu cầu
        public async Task<(bool Success, string? ErrorMessage)> TuChoiAsync(int id, TuChoiPhieuDto dto)
        {
            var phieu = await _repository.GetByIdAsync(id);
            if (phieu == null) return (false, "Không tìm thấy phiếu điều chuyển.");

            if (phieu.TrangThai != "ChoDuyet")
                return (false, "Chỉ có thể từ chối phiếu đang ở trạng thái 'Chờ duyệt'.");

            if (string.IsNullOrWhiteSpace(dto.LyDoTuChoi))
                return (false, "Vui lòng nhập lý do từ chối.");

            var success = await _repository.TuChoiAsync(id, dto.LyDoTuChoi);
            return success ? (true, null) : (false, "Không thể từ chối phiếu.");
        }

        // BƯỚC 3: Nhân viên chi nhánh NGUỒN xác nhận đã xuất kho thật
        public async Task<(bool Success, string? ErrorMessage)> XacNhanXuatKhoAsync(int id, XacNhanThucHienDto dto)
        {
            var phieu = await _repository.GetByIdAsync(id);
            if (phieu == null) return (false, "Không tìm thấy phiếu điều chuyển.");

            if (phieu.TrangThai != "DaDuyet")
                return (false, "Phiếu cần được duyệt trước khi xuất kho.");

            if (!await _repository.NhanVienExistsAsync(dto.NhanVienId))
                return (false, "Nhân viên không tồn tại.");

            try
            {
                var success = await _repository.XacNhanXuatKhoAsync(id, dto.NhanVienId);
                return success ? (true, null) : (false, "Không thể xác nhận xuất kho.");
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message);
            }
        }

        // BƯỚC 4: Nhân viên chi nhánh ĐÍCH xác nhận đã nhận hàng thật
        public async Task<(bool Success, string? ErrorMessage)> XacNhanNhanHangAsync(int id, XacNhanThucHienDto dto)
        {
            var phieu = await _repository.GetByIdAsync(id);
            if (phieu == null) return (false, "Không tìm thấy phiếu điều chuyển.");

            if (phieu.TrangThai != "DangVanChuyen")
                return (false, "Phiếu cần được xuất kho trước khi xác nhận nhận hàng.");

            if (!await _repository.NhanVienExistsAsync(dto.NhanVienId))
                return (false, "Nhân viên không tồn tại.");

            var success = await _repository.XacNhanNhanHangAsync(id, dto.NhanVienId);
            return success ? (true, null) : (false, "Không thể xác nhận nhận hàng.");
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