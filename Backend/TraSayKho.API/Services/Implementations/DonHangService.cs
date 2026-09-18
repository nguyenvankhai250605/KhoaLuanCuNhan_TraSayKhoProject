using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _repository;
        private readonly IHanMucService _hanMucService;
        private readonly IBacGiamGiaRepository _bacGiamGiaRepository;

        private static readonly Dictionary<string, string[]>
            QuyTacChuyenTrangThai = new()
            {
                ["ChoXacNhan"] = new[] { "DangXuLy", "DaHuy" },
                ["DangXuLy"] = new[] { "DangGiao", "DaHuy" },
                ["DangGiao"] = new[] { "DaGiao", "DaHuy" },
                ["DaGiao"] = new[] { "HoanThanh" },
                ["HoanThanh"] = Array.Empty<string>(),
                ["DaHuy"] = Array.Empty<string>()
            };

        public DonHangService(
            IDonHangRepository repository,
            IHanMucService hanMucService,
            IBacGiamGiaRepository bacGiamGiaRepository)
        {
            _repository = repository;
            _hanMucService = hanMucService;
            _bacGiamGiaRepository = bacGiamGiaRepository;
        }

        public async Task<List<DonHangDto>> GetAllAsync()
        {
            var danhSach = await _repository.GetAllAsync();

            return danhSach.Select(donHang => new DonHangDto
            {
                DonHangId = donHang.DonHangId,
                ChiNhanhId = donHang.ChiNhanhId,
                TenChiNhanh = donHang.ChiNhanh?.TenChiNhanh,
                TenKhachHang = donHang.KhachHang.HoTen,
                TrangThai = donHang.TrangThai.TenTrangThai,
                TongTien = donHang.TongTien,
                NgayDatHang = donHang.NgayDatHang
            }).ToList();
        }

        public async Task<DonHangChiTietDto?> GetByIdAsync(int id)
        {
            var donHang =
                await _repository.GetByIdWithDetailsAsync(id);

            return donHang == null
                ? null
                : MapChiTietToDto(donHang);
        }

        public async Task<(bool Success, string? ErrorMessage)>
            CapNhatTrangThaiAsync(
                int id,
                CapNhatTrangThaiDto dto)
        {
            var donHang =
                await _repository.GetByIdWithDetailsAsync(id);

            if (donHang == null)
            {
                return (false, "Không tìm thấy đơn hàng.");
            }

            if (string.IsNullOrWhiteSpace(dto.TenTrangThaiMoi))
            {
                return (false, "Vui lòng chọn trạng thái mới.");
            }

            var trangThaiMoi =
                await _repository.GetTrangThaiByTenAsync(
                    dto.TenTrangThaiMoi);

            if (trangThaiMoi == null)
            {
                return (false, "Trạng thái không hợp lệ.");
            }

            var tenTrangThaiHienTai =
                donHang.TrangThai.TenTrangThai;

            if (!QuyTacChuyenTrangThai.TryGetValue(
                    tenTrangThaiHienTai,
                    out var cacBuocDuocPhep) ||
                !cacBuocDuocPhep.Contains(
                    dto.TenTrangThaiMoi))
            {
                return (
                    false,
                    $"Không thể chuyển từ trạng thái " +
                    $"'{tenTrangThaiHienTai}' sang " +
                    $"'{dto.TenTrangThaiMoi}'.");
            }

            var thanhCong =
                await _repository.UpdateTrangThaiAsync(
                    id,
                    trangThaiMoi.TrangThaiId);

            return thanhCong
                ? (true, null)
                : (false, "Cập nhật trạng thái thất bại.");
        }

        // =========================================================
        // TẠO ĐƠN HÀNG
        // - Kiểm tra hạn mức.
        // - Chọn đơn vị sản phẩm theo FEFO.
        // - Tính giá xả kho theo từng lô.
        // - Nếu lấy hàng từ nhiều lô thì tách chi tiết theo lô.
        // - Áp dụng mã khuyến mãi cho toàn đơn.
        // =========================================================
        public async Task<(
            bool Success,
            string? ErrorMessage,
            DonHangChiTietDto? Result)> TaoDonHangAsync(
                DonHangCreateDto dto)
        {
            // =====================================================
            // BƯỚC 1: KIỂM TRA DỮ LIỆU CƠ BẢN
            // =====================================================
            if (!await _repository.KhachHangExistsAsync(
                    dto.KhachHangId))
            {
                return (
                    false,
                    "Khách hàng không tồn tại.",
                    null);
            }

            if (!await _repository.ChiNhanhExistsAsync(
                    dto.ChiNhanhId))
            {
                return (
                    false,
                    "Chi nhánh không tồn tại.",
                    null);
            }

            if (string.IsNullOrWhiteSpace(dto.DiaChiGiaoHang) ||
                string.IsNullOrWhiteSpace(dto.SoDienThoaiNhan))
            {
                return (
                    false,
                    "Vui lòng nhập đầy đủ địa chỉ và " +
                    "số điện thoại nhận hàng.",
                    null);
            }

            if (dto.ChiTiet == null ||
                dto.ChiTiet.Count == 0)
            {
                return (
                    false,
                    "Đơn hàng phải có ít nhất 1 sản phẩm.",
                    null);
            }

            if (dto.ChiTiet.Any(ct => ct.SoLuong <= 0))
            {
                return (
                    false,
                    "Số lượng sản phẩm phải lớn hơn 0.",
                    null);
            }

            // Gộp các dòng có cùng sản phẩm để không chọn trùng
            // một đơn vị sản phẩm trong cùng đơn hàng.
            var cacDongDat = dto.ChiTiet
                .GroupBy(ct => ct.SanPhamId)
                .Select(nhom => new
                {
                    SanPhamId = nhom.Key,
                    SoLuong = nhom.Sum(ct => ct.SoLuong)
                })
                .ToList();

            var cacBacGiamGia =
                await _bacGiamGiaRepository
                    .GetDangHoatDongAsync();

            var homNay =
                DateOnly.FromDateTime(DateTime.Now);

            var chiTiets =
                new List<ChiTietDonHang>();

            var danhSachDonViBan =
                new Dictionary<
                    ChiTietDonHang,
                    List<DonViSanPham>>();

            decimal tienHang = 0;

            // =====================================================
            // BƯỚC 2: XỬ LÝ TỪNG SẢN PHẨM
            // =====================================================
            foreach (var dongDat in cacDongDat)
            {
                var sanPham =
                    await _repository.GetSanPhamAsync(
                        dongDat.SanPhamId);

                if (sanPham == null)
                {
                    return (
                        false,
                        $"Sản phẩm ID {dongDat.SanPhamId} " +
                        "không tồn tại.",
                        null);
                }

                if (sanPham.TrangThai != "DangBan")
                {
                    return (
                        false,
                        $"Sản phẩm '{sanPham.TenSanPham}' " +
                        "hiện không còn bán.",
                        null);
                }

                var (hopLe, lyDoTuChoi) =
                    await _hanMucService
                        .KiemTraHanMucBanAsync(
                            dongDat.SanPhamId,
                            dto.ChiNhanhId,
                            dongDat.SoLuong);

                if (!hopLe)
                {
                    return (
                        false,
                        lyDoTuChoi,
                        null);
                }

                // Danh sách đã được Repository sắp xếp FEFO.
                var cacDonViConKho =
                    await _repository
                        .GetDonViSanPhamConKhoTheoFefoAsync(
                            dongDat.SanPhamId,
                            dto.ChiNhanhId);

                // Không bán đơn vị thuộc lô đã hết hạn.
                cacDonViConKho = cacDonViConKho
                    .Where(dv =>
                        dv.Thung.LoHang.HanSuDung >= homNay)
                    .ToList();

                var tenDonVi = string.IsNullOrWhiteSpace(
                    sanPham.DonViTinh)
                    ? "đơn vị"
                    : sanPham.DonViTinh.ToLower();

                if (cacDonViConKho.Count < dongDat.SoLuong)
                {
                    return (
                        false,
                        $"Sản phẩm '{sanPham.TenSanPham}' " +
                        $"tại chi nhánh chỉ còn " +
                        $"{cacDonViConKho.Count} {tenDonVi} " +
                        $"hợp lệ, không đủ đáp ứng " +
                        $"{dongDat.SoLuong} {tenDonVi}.",
                        null);
                }

                // FEFO: lấy số lượng đơn vị đầu tiên.
                var cacDonViDuocChon = cacDonViConKho
                    .Take(dongDat.SoLuong)
                    .ToList();

                // Một sản phẩm có thể được lấy từ nhiều lô.
                // Mỗi lô có thể có mức giảm giá khác nhau.
                var cacNhomTheoLo = cacDonViDuocChon
                    .GroupBy(dv => dv.Thung.LoHangId)
                    .ToList();

                foreach (var nhomLo in cacNhomTheoLo)
                {
                    var cacDonViTrongLo =
                        nhomLo.ToList();

                    var loHang =
                        cacDonViTrongLo[0].Thung.LoHang;

                    var mucGiamXaKho =
                        TinhMucGiamXaKhoTuDong(
                            loHang,
                            sanPham.DanhMucId,
                            cacBacGiamGia,
                            homNay);

                    var giaBanThucTe =
                        TinhGiaSauGiam(
                            sanPham.GiaBan,
                            mucGiamXaKho);

                    var chiTiet =
                        new ChiTietDonHang
                        {
                            SanPhamId =
                                sanPham.SanPhamId,

                            SoLuong =
                                cacDonViTrongLo.Count,

                            DonGia =
                                giaBanThucTe
                        };

                    chiTiets.Add(chiTiet);

                    danhSachDonViBan.Add(
                        chiTiet,
                        cacDonViTrongLo);

                    tienHang +=
                        giaBanThucTe *
                        cacDonViTrongLo.Count;
                }
            }

            // =====================================================
            // BƯỚC 3: XỬ LÝ MÃ KHUYẾN MÃI
            // =====================================================
            KhuyenMai? khuyenMaiApDung = null;
            decimal tienGiamGia = 0;

            if (!string.IsNullOrWhiteSpace(
                    dto.MaKhuyenMai))
            {
                var maCode =
                    dto.MaKhuyenMai.Trim();

                var khuyenMai =
                    await _repository
                        .GetKhuyenMaiByMaCodeAsync(maCode);

                if (khuyenMai == null)
                {
                    return (
                        false,
                        "Mã khuyến mãi không tồn tại.",
                        null);
                }

                if (!khuyenMai.TrangThai)
                {
                    return (
                        false,
                        "Mã khuyến mãi đã ngừng hoạt động.",
                        null);
                }

                var bayGio = DateTime.Now;

                if (bayGio < khuyenMai.NgayBatDau ||
                    bayGio > khuyenMai.NgayKetThuc)
                {
                    return (
                        false,
                        "Mã khuyến mãi không còn trong " +
                        "thời gian áp dụng.",
                        null);
                }

                if (khuyenMai.SoLuotDaSuDung >=
                    khuyenMai.SoLuotSuDungToiDa)
                {
                    return (
                        false,
                        "Mã khuyến mãi đã hết lượt sử dụng.",
                        null);
                }

                if (tienHang <
                    khuyenMai.GiaTriDonHangToiThieu)
                {
                    return (
                        false,
                        $"Đơn hàng cần tối thiểu " +
                        $"{khuyenMai.GiaTriDonHangToiThieu:N0}đ " +
                        "để áp dụng mã này.",
                        null);
                }

                tienGiamGia =
                    khuyenMai.LoaiGiam == "PhanTram"
                        ? tienHang *
                          khuyenMai.GiaTriGiam / 100m
                        : khuyenMai.GiaTriGiam;

                if (tienGiamGia > tienHang)
                {
                    tienGiamGia = tienHang;
                }

                if (tienGiamGia < 0)
                {
                    tienGiamGia = 0;
                }

                khuyenMaiApDung = khuyenMai;
            }

            var tongTien =
                tienHang - tienGiamGia;

            // =====================================================
            // BƯỚC 4: LẤY TRẠNG THÁI KHỞI TẠO
            // =====================================================
            var trangThaiKhoiTao =
                await _repository.GetTrangThaiByTenAsync(
                    "ChoXacNhan");

            if (trangThaiKhoiTao == null)
            {
                return (
                    false,
                    "Lỗi hệ thống: không tìm thấy trạng thái " +
                    "khởi tạo 'ChoXacNhan'.",
                    null);
            }

            // =====================================================
            // BƯỚC 5: TẠO ĐƠN HÀNG
            // =====================================================
            var donHang = new DonHang
            {
                KhachHangId = dto.KhachHangId,
                ChiNhanhId = dto.ChiNhanhId,
                TrangThaiId =
                    trangThaiKhoiTao.TrangThaiId,

                KhuyenMaiId =
                    khuyenMaiApDung?.KhuyenMaiId,

                DiaChiGiaoHang =
                    dto.DiaChiGiaoHang.Trim(),

                SoDienThoaiNhan =
                    dto.SoDienThoaiNhan.Trim(),

                PhuongThucThanhToan =
                    string.IsNullOrWhiteSpace(
                        dto.PhuongThucThanhToan)
                        ? "COD"
                        : dto.PhuongThucThanhToan.Trim(),

                TienHang = tienHang,
                TienGiamGia = tienGiamGia,
                TongTien = tongTien,

                GhiChu =
                    string.IsNullOrWhiteSpace(dto.GhiChu)
                        ? null
                        : dto.GhiChu.Trim(),

                NgayDatHang = DateTime.Now
            };

            var donHangDaTao =
                await _repository.TaoDonHangAsync(
                    donHang,
                    chiTiets,
                    danhSachDonViBan,
                    khuyenMaiApDung);

            var ketQua =
                await _repository.GetByIdWithDetailsAsync(
                    donHangDaTao.DonHangId);

            if (ketQua == null)
            {
                return (
                    false,
                    "Đơn hàng đã được tạo nhưng không thể " +
                    "tải lại dữ liệu chi tiết.",
                    null);
            }

            return (
                true,
                null,
                MapChiTietToDto(ketQua));
        }

        // =========================================================
        // TÍNH MỨC GIẢM GIÁ XẢ KHO
        // =========================================================
        private static decimal TinhMucGiamXaKhoTuDong(
            LoHang loHang,
            int danhMucId,
            IReadOnlyCollection<BacGiamGiaXaKho>
                cacBacDangHoatDong,
            DateOnly homNay)
        {
            var tongSoNgaySuDung =
                loHang.HanSuDung.DayNumber -
                loHang.NgaySanXuat.DayNumber;

            if (tongSoNgaySuDung <= 0)
                return 0;

            var soNgayConLai = Math.Max(
                0,
                loHang.HanSuDung.DayNumber -
                homNay.DayNumber);

            var phanTramConLai =
                (decimal)soNgayConLai /
                tongSoNgaySuDung * 100m;

            // Ưu tiên bậc giảm riêng của danh mục.
            var bacRiengDanhMuc =
                cacBacDangHoatDong
                    .Where(bac =>
                        bac.TrangThai &&
                        bac.DanhMucId == danhMucId &&
                        phanTramConLai <=
                        bac.PhanTramThoiGianConLaiToiDa)
                    .OrderBy(bac =>
                        bac.PhanTramThoiGianConLaiToiDa)
                    .FirstOrDefault();

            if (bacRiengDanhMuc != null)
            {
                return bacRiengDanhMuc
                    .MucGiamGiaPhanTram;
            }

            // Nếu không có bậc riêng thì dùng bậc chung.
            var bacChung =
                cacBacDangHoatDong
                    .Where(bac =>
                        bac.TrangThai &&
                        bac.DanhMucId == null &&
                        phanTramConLai <=
                        bac.PhanTramThoiGianConLaiToiDa)
                    .OrderBy(bac =>
                        bac.PhanTramThoiGianConLaiToiDa)
                    .FirstOrDefault();

            return bacChung?.MucGiamGiaPhanTram ?? 0;
        }

        private static decimal TinhGiaSauGiam(
            decimal giaGoc,
            decimal mucGiamPhanTram)
        {
            if (mucGiamPhanTram <= 0)
                return giaGoc;

            var mucGiamHopLe =
                Math.Min(mucGiamPhanTram, 100m);

            var giaSauGiam =
                giaGoc *
                (1m - mucGiamHopLe / 100m);

            return Math.Round(
                giaSauGiam,
                0,
                MidpointRounding.AwayFromZero);
        }

        private static DonHangChiTietDto MapChiTietToDto(
            DonHang donHang)
        {
            return new DonHangChiTietDto
            {
                DonHangId = donHang.DonHangId,
                ChiNhanhId = donHang.ChiNhanhId,
                TenChiNhanh =
                    donHang.ChiNhanh?.TenChiNhanh,

                TenKhachHang =
                    donHang.KhachHang.HoTen,

                TrangThai =
                    donHang.TrangThai.TenTrangThai,

                DiaChiGiaoHang =
                    donHang.DiaChiGiaoHang,

                TongTien = donHang.TongTien,
                NgayDatHang = donHang.NgayDatHang,

                ChiTietSanPhams =
                    donHang.ChiTietDonHangs
                        .Select(chiTiet =>
                            new ChiTietSanPhamTrongDonDto
                            {
                                TenSanPham =
                                    chiTiet.SanPham.TenSanPham,

                                SoLuong =
                                    chiTiet.SoLuong,

                                DonGia =
                                    chiTiet.DonGia,

                                ThanhTien =
                                    chiTiet.ThanhTien ??
                                    (chiTiet.DonGia *
                                     chiTiet.SoLuong)
                            })
                        .ToList()
            };
        }
    }
}