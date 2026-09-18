using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class PhieuDieuChuyenService
        : IPhieuDieuChuyenService
    {
        private readonly
            IPhieuDieuChuyenRepository
            _repository;

        public PhieuDieuChuyenService(
            IPhieuDieuChuyenRepository repository)
        {
            _repository = repository;
        }

        public async Task<
            List<PhieuDieuChuyenDto>>
            GetAllAsync()
        {
            var list =
                await _repository.GetAllAsync();

            return list
                .Select(MapToDto)
                .ToList();
        }

        public async Task<PhieuDieuChuyenDto?>
            GetByIdAsync(int id)
        {
            var phieu =
                await _repository
                    .GetByIdAsync(id);

            return phieu == null
                ? null
                : MapToDto(phieu);
        }

        public async Task<(
            bool Success,
            string? ErrorMessage,
            PhieuDieuChuyenDto? Result)>
            TaoYeuCauAsync(
                PhieuDieuChuyenCreateDto dto)
        {
            if (dto.ChiNhanhGuiId ==
                dto.ChiNhanhNhanId)
            {
                return (
                    false,
                    "Chi nhánh nguồn và chi nhánh nhận phải khác nhau.",
                    null);
            }

            if (!await _repository
                .ChiNhanhExistsAsync(
                    dto.ChiNhanhGuiId))
            {
                return (
                    false,
                    "Chi nhánh nguồn không tồn tại hoặc đã ngừng hoạt động.",
                    null);
            }

            if (!await _repository
                .ChiNhanhExistsAsync(
                    dto.ChiNhanhNhanId))
            {
                return (
                    false,
                    "Chi nhánh nhận không tồn tại hoặc đã ngừng hoạt động.",
                    null);
            }

            if (!await _repository
                .NhanVienExistsAsync(
                    dto.NhanVienTaoId))
            {
                return (
                    false,
                    "Người tạo yêu cầu không tồn tại.",
                    null);
            }

            if (dto.ChiTiet == null ||
                dto.ChiTiet.Count == 0)
            {
                return (
                    false,
                    "Yêu cầu điều chuyển phải có ít nhất một sản phẩm.",
                    null);
            }

            if (dto.ChiTiet.Any(ct =>
                    ct.SoLuong <= 0))
            {
                return (
                    false,
                    "Số lượng điều chuyển phải lớn hơn 0.",
                    null);
            }

            // Gộp các dòng trùng sản phẩm để
            // tránh chọn cùng một thùng nhiều lần.
            var cacDongDieuChuyen =
                dto.ChiTiet
                    .GroupBy(ct =>
                        ct.SanPhamId)
                    .Select(g => new
                    {
                        SanPhamId =
                            g.Key,

                        SoLuong =
                            g.Sum(x =>
                                x.SoLuong)
                    })
                    .ToList();

            var chiTiets =
                new List<
                    ChiTietPhieuDieuChuyen>();

            foreach (var dong
                     in cacDongDieuChuyen)
            {
                var cacThung =
                    await _repository
                        .GetThungHangTheoFefoAsync(
                            dong.SanPhamId,
                            dto.ChiNhanhGuiId);

                if (cacThung.Count == 0)
                {
                    return (
                        false,
                        $"Chi nhánh nguồn không còn thùng nào của sản phẩm ID {dong.SanPhamId}.",
                        null);
                }

                // Dùng số đơn vị còn kho thực tế,
                // không dùng sức chứa ban đầu.
                var tongSoDonViConKho =
                    cacThung.Sum(t =>
                        t.DonViSanPhams.Count(dv =>
                            dv.TrangThai ==
                                "ConKho"));

                if (tongSoDonViConKho <
                    dong.SoLuong)
                {
                    return (
                        false,
                        $"Sản phẩm ID {dong.SanPhamId} chỉ còn {tongSoDonViConKho} đơn vị tại chi nhánh nguồn, không đủ {dong.SoLuong} đơn vị.",
                        null);
                }

                // Điều chuyển nguyên thùng.
                // Vì vậy tổng thực tế có thể bằng
                // hoặc lớn hơn số lượng yêu cầu.
                var soLuongDaChon = 0;

                foreach (var thung in cacThung)
                {
                    var soDonViConKhoTrongThung =
                        thung.DonViSanPhams
                            .Count(dv =>
                                dv.TrangThai ==
                                    "ConKho");

                    if (soDonViConKhoTrongThung <= 0)
                        continue;

                    chiTiets.Add(
                        new ChiTietPhieuDieuChuyen
                        {
                            ThungId =
                                thung.ThungId
                        });

                    soLuongDaChon +=
                        soDonViConKhoTrongThung;

                    if (soLuongDaChon >=
                        dong.SoLuong)
                    {
                        break;
                    }
                }
            }

            var phieu =
                new PhieuDieuChuyenKho
                {
                    ChiNhanhGuiId =
                        dto.ChiNhanhGuiId,

                    ChiNhanhNhanId =
                        dto.ChiNhanhNhanId,

                    NhanVienTaoId =
                        dto.NhanVienTaoId,

                    GhiChu =
                        string.IsNullOrWhiteSpace(
                            dto.GhiChu)
                            ? null
                            : dto.GhiChu.Trim(),

                    TrangThai =
                        "ChoDuyet",

                    NgayTao =
                        DateTime.Now
                };

            var created =
                await _repository
                    .TaoYeuCauAsync(
                        phieu,
                        chiTiets);

            return (
                true,
                null,
                MapToDto(created));
        }

        public async Task<(
            bool Success,
            string? ErrorMessage)>
            DuyetAsync(int id)
        {
            var phieu =
                await _repository
                    .GetByIdAsync(id);

            if (phieu == null)
            {
                return (
                    false,
                    "Không tìm thấy phiếu điều chuyển.");
            }

            if (phieu.TrangThai !=
                "ChoDuyet")
            {
                return (
                    false,
                    $"Chỉ có thể duyệt phiếu đang chờ duyệt. Trạng thái hiện tại là '{phieu.TrangThai}'.");
            }

            var success =
                await _repository
                    .DuyetAsync(id);

            return success
                ? (true, null)
                : (
                    false,
                    "Không thể duyệt phiếu.");
        }

        public async Task<(
            bool Success,
            string? ErrorMessage)>
            TuChoiAsync(
                int id,
                TuChoiPhieuDto dto)
        {
            var phieu =
                await _repository
                    .GetByIdAsync(id);

            if (phieu == null)
            {
                return (
                    false,
                    "Không tìm thấy phiếu điều chuyển.");
            }

            if (phieu.TrangThai !=
                "ChoDuyet")
            {
                return (
                    false,
                    "Chỉ có thể từ chối phiếu đang chờ duyệt.");
            }

            if (string.IsNullOrWhiteSpace(
                dto.LyDoTuChoi))
            {
                return (
                    false,
                    "Vui lòng nhập lý do từ chối.");
            }

            var success =
                await _repository
                    .TuChoiAsync(
                        id,
                        dto.LyDoTuChoi.Trim());

            return success
                ? (true, null)
                : (
                    false,
                    "Không thể từ chối phiếu.");
        }

        public async Task<(
            bool Success,
            string? ErrorMessage)>
            XacNhanXuatKhoAsync(
                int id,
                XacNhanThucHienDto dto)
        {
            var phieu =
                await _repository
                    .GetByIdAsync(id);

            if (phieu == null)
            {
                return (
                    false,
                    "Không tìm thấy phiếu điều chuyển.");
            }

            if (phieu.TrangThai !=
                "DaDuyet")
            {
                return (
                    false,
                    "Phiếu cần được duyệt trước khi xuất kho.");
            }

            if (!await _repository
                .NhanVienExistsAsync(
                    dto.NhanVienId))
            {
                return (
                    false,
                    "Nhân viên không tồn tại.");
            }

            try
            {
                var success =
                    await _repository
                        .XacNhanXuatKhoAsync(
                            id,
                            dto.NhanVienId);

                return success
                    ? (true, null)
                    : (
                        false,
                        "Không thể xác nhận xuất kho.");
            }
            catch (InvalidOperationException ex)
            {
                return (
                    false,
                    ex.Message);
            }
        }

        public async Task<(
            bool Success,
            string? ErrorMessage)>
            XacNhanNhanHangAsync(
                int id,
                XacNhanThucHienDto dto)
        {
            var phieu =
                await _repository
                    .GetByIdAsync(id);

            if (phieu == null)
            {
                return (
                    false,
                    "Không tìm thấy phiếu điều chuyển.");
            }

            if (phieu.TrangThai !=
                "DangVanChuyen")
            {
                return (
                    false,
                    "Phiếu cần được xuất kho trước khi xác nhận nhận hàng.");
            }

            if (!await _repository
                .NhanVienExistsAsync(
                    dto.NhanVienId))
            {
                return (
                    false,
                    "Nhân viên không tồn tại.");
            }

            var success =
                await _repository
                    .XacNhanNhanHangAsync(
                        id,
                        dto.NhanVienId);

            return success
                ? (true, null)
                : (
                    false,
                    "Không thể xác nhận nhận hàng.");
        }

        private static PhieuDieuChuyenDto
            MapToDto(
                PhieuDieuChuyenKho phieu)
        {
            return new PhieuDieuChuyenDto
            {
                PhieuDieuChuyenId =
                    phieu.PhieuDieuChuyenId,

                ChiNhanhGuiId =
                    phieu.ChiNhanhGuiId,

                TenChiNhanhGui =
                    phieu.ChiNhanhGui
                        .TenChiNhanh,

                ChiNhanhNhanId =
                    phieu.ChiNhanhNhanId,

                TenChiNhanhNhan =
                    phieu.ChiNhanhNhan
                        .TenChiNhanh,

                TenNhanVienTao =
                    phieu.NhanVienTao.HoTen,

                TenNhanVienXacNhan =
                    phieu.NhanVienXacNhan
                        ?.HoTen,

                TrangThai =
                    phieu.TrangThai,

                GhiChu =
                    phieu.GhiChu,

                NgayTao =
                    phieu.NgayTao,

                NgayXacNhan =
                    phieu.NgayXacNhan,

                ChiTiet =
                    phieu
                        .ChiTietPhieuDieuChuyens
                        .Select(ct =>
                            new ChiTietPhieuDieuChuyenDto
                            {
                                ChiTietId =
                                    ct.ChiTietId,

                                ThungId =
                                    ct.ThungId,

                                MaThung =
                                    ct.Thung.MaThung,

                                LoHangId =
                                    ct.Thung.LoHangId,

                                SoLo =
                                    ct.Thung.LoHang.SoLo,

                                SanPhamId =
                                    ct.Thung.LoHang
                                        .SanPhamId,

                                TenSanPham =
                                    ct.Thung.LoHang
                                        .SanPham
                                        .TenSanPham,

                                DonViTinh =
                                    ct.Thung.LoHang
                                        .SanPham
                                        .DonViTinh,

                                HanSuDung =
                                    ct.Thung.LoHang
                                        .HanSuDung,

                                // Số đơn vị còn kho thực tế
                                // được chuyển cùng thùng.
                                SoLuongDonVi =
                                    ct.Thung
                                        .DonViSanPhams
                                        .Count(dv =>
                                            dv.TrangThai ==
                                                "ConKho")
                            })
                        .ToList()
            };
        }
    }
}