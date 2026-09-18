using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class LoHangService : ILoHangService
    {
        private readonly ILoHangRepository _repository;

        private readonly IBacGiamGiaRepository
            _bacGiamGiaRepository;

        public LoHangService(
            ILoHangRepository repository,
            IBacGiamGiaRepository
                bacGiamGiaRepository)
        {
            _repository = repository;

            _bacGiamGiaRepository =
                bacGiamGiaRepository;
        }

        public async Task<List<LoHangDto>>
            GetAllAsync()
        {
            var danhSachLo =
                await _repository.GetAllAsync();

            var cacBacGiamGia =
                await _bacGiamGiaRepository
                    .GetDangHoatDongAsync();

            return danhSachLo
                .Select(lh =>
                    MapToDto(
                        lh,
                        cacBacGiamGia))
                .ToList();
        }

        public async Task<LoHangDto?>
            GetByIdAsync(int id)
        {
            var loHang =
                await _repository
                    .GetByIdAsync(id);

            if (loHang == null)
                return null;

            var cacBacGiamGia =
                await _bacGiamGiaRepository
                    .GetDangHoatDongAsync();

            return MapToDto(
                loHang,
                cacBacGiamGia);
        }

        public async Task<List<LoHangDto>>
            GetBySanPhamAsync(
                int sanPhamId)
        {
            var danhSachLo =
                await _repository
                    .GetBySanPhamAsync(
                        sanPhamId);

            var cacBacGiamGia =
                await _bacGiamGiaRepository
                    .GetDangHoatDongAsync();

            return danhSachLo
                .Select(lh =>
                    MapToDto(
                        lh,
                        cacBacGiamGia))
                .ToList();
        }

        public async Task<List<LoHangDto>>
            GetSapHetHanAsync(
                int phanTramNguong)
        {
            if (phanTramNguong <= 0)
                phanTramNguong = 25;

            if (phanTramNguong > 100)
                phanTramNguong = 100;

            var danhSachLo =
                await _repository
                    .GetSapHetHanAsync();

            var cacBacGiamGia =
                await _bacGiamGiaRepository
                    .GetDangHoatDongAsync();

            var homNay =
                DateOnly.FromDateTime(
                    DateTime.Now);

            var result =
                new List<LoHangDto>();

            foreach (var loHang in danhSachLo)
            {
                var tongSoNgaySuDung =
                    loHang.HanSuDung.DayNumber -
                    loHang.NgaySanXuat.DayNumber;

                if (tongSoNgaySuDung <= 0)
                    continue;

                var soNgayConLai =
                    loHang.HanSuDung.DayNumber -
                    homNay.DayNumber;

                // Lô đã hết hạn không được bán.
                if (soNgayConLai < 0)
                    continue;

                var phanTramConLai =
                    (decimal)soNgayConLai /
                    tongSoNgaySuDung *
                    100m;

                if (phanTramConLai <=
                    phanTramNguong)
                {
                    result.Add(
                        MapToDto(
                            loHang,
                            cacBacGiamGia));
                }
            }

            return result;
        }

        public async Task<(
            bool Success,
            string? ErrorMessage,
            LoHangDto? Result)>
            CreateAsync(
                LoHangCreateDto dto,
                int? chiNhanhNguoiGoi)
        {
            if (!await _repository
                .SanPhamExistsAsync(
                    dto.SanPhamId))
            {
                return (
                    false,
                    "Sản phẩm không tồn tại.",
                    null);
            }

            var soLo =
                dto.SoLo?.Trim();

            if (string.IsNullOrWhiteSpace(soLo))
            {
                return (
                    false,
                    "Vui lòng nhập số lô.",
                    null);
            }

            if (await _repository
                .SoLoExistsAsync(soLo))
            {
                return (
                    false,
                    "Số lô này đã tồn tại, vui lòng dùng số lô khác.",
                    null);
            }

            if (dto.HanSuDung <=
                dto.NgaySanXuat)
            {
                return (
                    false,
                    "Hạn sử dụng phải sau ngày sản xuất.",
                    null);
            }

            if (dto.SoLuongThung <= 0 ||
                dto.SoDonViMoiThung <= 0)
            {
                return (
                    false,
                    "Số lượng thùng và số đơn vị mỗi thùng phải lớn hơn 0.",
                    null);
            }

            var chiNhanhChinh =
                await _repository
                    .GetTruSoChinhAsync();

            if (chiNhanhChinh == null)
            {
                return (
                    false,
                    "Hệ thống chưa cấu hình chi nhánh chính.",
                    null);
            }

            // Admin không bị giới hạn chi nhánh.
            // Chủ cửa hàng phải thuộc chi nhánh chính.
            if (chiNhanhNguoiGoi.HasValue &&
                chiNhanhNguoiGoi.Value !=
                    chiNhanhChinh.ChiNhanhId)
            {
                return (
                    false,
                    "Chỉ chi nhánh chính mới được phép nhập lô hàng mới. Các chi nhánh khác nhận hàng qua phiếu điều chuyển kho.",
                    null);
            }

            int tongSoLuongNhap;

            try
            {
                tongSoLuongNhap = checked(
                    dto.SoLuongThung *
                    dto.SoDonViMoiThung);
            }
            catch (OverflowException)
            {
                return (
                    false,
                    "Tổng số lượng nhập vượt quá giới hạn cho phép.",
                    null);
            }

            var loHang = new LoHang
            {
                SanPhamId =
                    dto.SanPhamId,

                SoLo =
                    soLo,

                NgaySanXuat =
                    dto.NgaySanXuat,

                HanSuDung =
                    dto.HanSuDung,

                TongSoLuongNhap =
                    tongSoLuongNhap,

                TrangThai =
                    "ConHang",

                NgayTao =
                    DateTime.Now
            };

            var created =
                await _repository
                    .TaoLoVaPhanBoAsync(
                        loHang,
                        chiNhanhChinh.ChiNhanhId,
                        dto.SoLuongThung,
                        dto.SoDonViMoiThung);

            await _repository
                .DongBoTonKhoSanPhamAsync(
                    dto.SanPhamId);

            var cacBacGiamGia =
                await _bacGiamGiaRepository
                    .GetDangHoatDongAsync();

            return (
                true,
                null,
                MapToDto(
                    created,
                    cacBacGiamGia));
        }

        public async Task<(
            bool Success,
            string? ErrorMessage,
            TruyXuatNguonGocDto? Result)>
            TruyXuatTheoMaDonViAsync(
                string maDonVi)
        {
            if (string.IsNullOrWhiteSpace(
                maDonVi))
            {
                return (
                    false,
                    "Vui lòng nhập mã đơn vị sản phẩm cần tra cứu.",
                    null);
            }

            var donVi =
                await _repository
                    .GetDonViSanPhamByMaAsync(
                        maDonVi.Trim());

            if (donVi == null)
            {
                return (
                    false,
                    "Không tìm thấy hộp hoặc gói trà với mã này.",
                    null);
            }

            var thung =
                donVi.Thung;

            var loHang =
                thung.LoHang;

            var donHang =
                donVi.ChiTietDonHang?.DonHang;

            var result =
                new TruyXuatNguonGocDto
                {
                    DonViId =
                        donVi.DonViId,

                    MaDonVi =
                        donVi.MaDonVi,

                    DonViTinh =
                        loHang.SanPham.DonViTinh,

                    TrangThaiDonVi =
                        donVi.TrangThai,

                    MaThung =
                        thung.MaThung,

                    ChiNhanhPhanBoId =
                        thung.ChiNhanhId,

                    TenChiNhanhPhanBo =
                        thung.ChiNhanh
                            .TenChiNhanh,

                    SoLo =
                        loHang.SoLo,

                    NgaySanXuat =
                        loHang.NgaySanXuat,

                    HanSuDung =
                        loHang.HanSuDung,

                    SanPhamId =
                        loHang.SanPhamId,

                    TenSanPham =
                        loHang.SanPham
                            .TenSanPham,

                    DonHangId =
                        donHang?.DonHangId,

                    NgayBan =
                        donVi.NgayBan,

                    TenKhachHangMua =
                        donHang?.KhachHang
                            .HoTen,

                    ChiNhanhBanId =
                        donHang?.ChiNhanhId,

                    TenChiNhanhBan =
                        donHang?.ChiNhanh
                            ?.TenChiNhanh
                };

            return (
                true,
                null,
                result);
        }

        private static LoHangDto MapToDto(
            LoHang loHang,
            IReadOnlyCollection<BacGiamGiaXaKho>
                cacBacDangHoatDong)
        {
            var homNay =
                DateOnly.FromDateTime(
                    DateTime.Now);

            var soNgayConLai =
                loHang.HanSuDung.DayNumber -
                homNay.DayNumber;

            decimal? mucGiamGia = null;
            var laGiamGiaTuDong = false;

            // Hàng hết hạn không được giảm giá
            // để tiếp tục bán.
            if (soNgayConLai >= 0)
            {
                var tongSoNgaySuDung =
                    loHang.HanSuDung.DayNumber -
                    loHang.NgaySanXuat.DayNumber;

                if (tongSoNgaySuDung > 0)
                {
                    var phanTramConLai =
                        (decimal)soNgayConLai /
                        tongSoNgaySuDung *
                        100m;

                    var bacRiengDanhMuc =
                        cacBacDangHoatDong
                            .Where(b =>
                                b.TrangThai &&
                                b.DanhMucId ==
                                    loHang.SanPham
                                        .DanhMucId &&
                                phanTramConLai <=
                                    b.PhanTramThoiGianConLaiToiDa)
                            .OrderBy(b =>
                                b.PhanTramThoiGianConLaiToiDa)
                            .FirstOrDefault();

                    var bacChung =
                        cacBacDangHoatDong
                            .Where(b =>
                                b.TrangThai &&
                                b.DanhMucId == null &&
                                phanTramConLai <=
                                    b.PhanTramThoiGianConLaiToiDa)
                            .OrderBy(b =>
                                b.PhanTramThoiGianConLaiToiDa)
                            .FirstOrDefault();

                    var bacApDung =
                        bacRiengDanhMuc ??
                        bacChung;

                    if (bacApDung != null)
                    {
                        mucGiamGia =
                            bacApDung
                                .MucGiamGiaPhanTram;

                        laGiamGiaTuDong =
                            true;
                    }
                }
            }

            var giaSauGiam =
                mucGiamGia.HasValue
                    ? loHang.SanPham.GiaBan *
                      (1m -
                       mucGiamGia.Value / 100m)
                    : loHang.SanPham.GiaBan;

            var tongSoLuongConKho =
                loHang.ThungHangs.Sum(t =>
                    t.DonViSanPhams.Count(dv =>
                        dv.TrangThai ==
                            "ConKho"));

            return new LoHangDto
            {
                LoHangId =
                    loHang.LoHangId,

                SanPhamId =
                    loHang.SanPhamId,

                TenSanPham =
                    loHang.SanPham.TenSanPham,

                DonViTinh =
                    loHang.SanPham.DonViTinh,

                SoLo =
                    loHang.SoLo,

                NgaySanXuat =
                    loHang.NgaySanXuat,

                HanSuDung =
                    loHang.HanSuDung,

                TongSoLuongNhap =
                    loHang.TongSoLuongNhap,

                TongSoLuongConKho =
                    tongSoLuongConKho,

                TrangThai =
                    loHang.TrangThai,

                SoNgayConLaiDenHan =
                    soNgayConLai,

                MucGiamGiaHienTai =
                    mucGiamGia,

                LaGiamGiaTuDong =
                    laGiamGiaTuDong,

                GiaSauGiam =
                    Math.Round(
                        giaSauGiam,
                        0,
                        MidpointRounding
                            .AwayFromZero),

                DanhSachThung =
                    loHang.ThungHangs
                        .OrderBy(t => t.MaThung)
                        .Select(t =>
                            new ThungHangDto
                            {
                                ThungId =
                                    t.ThungId,

                                MaThung =
                                    t.MaThung,

                                ChiNhanhId =
                                    t.ChiNhanhId,

                                TenChiNhanh =
                                    t.ChiNhanh
                                        .TenChiNhanh,

                                SoLuongDonVi =
                                    t.SoLuongDonVi,

                                SoLuongConKho =
                                    t.DonViSanPhams
                                        .Count(dv =>
                                            dv.TrangThai ==
                                                "ConKho"),

                                TrangThai =
                                    t.TrangThai
                            })
                        .ToList()
            };
        }
    }
}