-- =============================================
-- DATABASE: Quản lý bán trà sấy khô + Chatbot AI (Mô hình đa chi nhánh)
-- =============================================
CREATE DATABASE TraSayKhoDB;
GO

USE TraSayKhoDB;
GO

-- =============================================
-- 1. NHÓM: CHI NHÁNH
-- =============================================

CREATE TABLE ChiNhanh (
    ChiNhanhID INT IDENTITY(1,1) PRIMARY KEY,
    TenChiNhanh NVARCHAR(150) NOT NULL,
    DiaChi NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(15) NULL,
    LaTruSoChinh BIT NOT NULL DEFAULT 0,
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 2. NHÓM: TÀI KHOẢN & PHÂN QUYỀN
-- =============================================

CREATE TABLE VaiTro (
    VaiTroID INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE TaiKhoan (
    TaiKhoanID INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    MatKhauHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    SoDienThoai NVARCHAR(15),
    VaiTroID INT NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TaiKhoan_VaiTro FOREIGN KEY (VaiTroID) REFERENCES VaiTro(VaiTroID)
);
GO

CREATE TABLE KhachHang (
    KhachHangID INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanID INT NOT NULL UNIQUE,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL,
    DiaChi NVARCHAR(255) NULL,
    AvatarURL NVARCHAR(255) NULL,
    CONSTRAINT FK_KhachHang_TaiKhoan FOREIGN KEY (TaiKhoanID) REFERENCES TaiKhoan(TaiKhoanID)
);
GO

CREATE TABLE NhanVien (
    NhanVienID INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanID INT NOT NULL UNIQUE,
    ChiNhanhID INT NULL,                    -- NULL = Quản trị viên tổng (xem toàn hệ thống)
    HoTen NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NULL,
    NgayVaoLam DATE NULL,
    CONSTRAINT FK_NhanVien_TaiKhoan FOREIGN KEY (TaiKhoanID) REFERENCES TaiKhoan(TaiKhoanID),
    CONSTRAINT FK_NhanVien_ChiNhanh FOREIGN KEY (ChiNhanhID) REFERENCES ChiNhanh(ChiNhanhID)
);
GO

-- =============================================
-- 3. NHÓM: SẢN PHẨM
-- =============================================

CREATE TABLE DanhMuc (
    DanhMucID INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    DanhMucChaID INT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_DanhMuc_DanhMucCha FOREIGN KEY (DanhMucChaID) REFERENCES DanhMuc(DanhMucID)
);
GO

CREATE TABLE ThanhPhan (
    ThanhPhanID INT IDENTITY(1,1) PRIMARY KEY,
    TenThanhPhan NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(255) NULL
);
GO

CREATE TABLE CongDung (
    CongDungID INT IDENTITY(1,1) PRIMARY KEY,
    TenCongDung NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(255) NULL
);
GO

CREATE TABLE SanPham (
    SanPhamID INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(150) NOT NULL,
    DanhMucID INT NOT NULL,
    MoTaChiTiet NVARCHAR(MAX) NULL,
    XuatXu NVARCHAR(100) NULL,
    GiaBan DECIMAL(18,2) NOT NULL CHECK (GiaBan >= 0),
    SoLuongTon INT NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0),  -- Tự động đồng bộ = tổng SoLuongConLai của các LoHang
    DonViTinh NVARCHAR(20) NULL,
    HanSuDung DATE NULL,                     -- Tự động đồng bộ = HSD gần nhất trong các LoHang còn hàng
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangBan',
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (DanhMucID) REFERENCES DanhMuc(DanhMucID)
);
GO

CREATE TABLE SanPham_ThanhPhan (
    SanPhamID INT NOT NULL,
    ThanhPhanID INT NOT NULL,
    PRIMARY KEY (SanPhamID, ThanhPhanID),
    CONSTRAINT FK_SPTP_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID) ON DELETE CASCADE,
    CONSTRAINT FK_SPTP_ThanhPhan FOREIGN KEY (ThanhPhanID) REFERENCES ThanhPhan(ThanhPhanID)
);
GO

CREATE TABLE SanPham_CongDung (
    SanPhamID INT NOT NULL,
    CongDungID INT NOT NULL,
    PRIMARY KEY (SanPhamID, CongDungID),
    CONSTRAINT FK_SPCD_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID) ON DELETE CASCADE,
    CONSTRAINT FK_SPCD_CongDung FOREIGN KEY (CongDungID) REFERENCES CongDung(CongDungID)
);
GO

CREATE TABLE HinhAnhSanPham (
    HinhAnhID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    DuongDanAnh NVARCHAR(255) NOT NULL,
    ThuTuHienThi INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_HinhAnh_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID) ON DELETE CASCADE
);
GO

-- =============================================
-- 4. NHÓM: LÔ HÀNG (theo Chi nhánh, hỗ trợ FEFO + xả kho)
-- =============================================

CREATE TABLE LoHang (
    LoHangID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    ChiNhanhID INT NOT NULL,
    SoLo NVARCHAR(50) NOT NULL,
    NgayNhap DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    HanSuDung DATE NOT NULL,
    SoLuongNhap INT NOT NULL CHECK (SoLuongNhap > 0),
    SoLuongConLai INT NOT NULL CHECK (SoLuongConLai >= 0),
    MucGiamGiaHienTai DECIMAL(5,2) NULL,        -- % giảm đang áp dụng (xả kho), NULL = giá gốc
    NgayBatDauApDungGiam DATE NULL,
    NgayKetThucApDungGiam DATE NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ConHang',  -- ConHang, HetHang, DaHetHan
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_LoHang_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID),
    CONSTRAINT FK_LoHang_ChiNhanh FOREIGN KEY (ChiNhanhID) REFERENCES ChiNhanh(ChiNhanhID),
    CONSTRAINT UQ_LoHang_SoLo UNIQUE (SanPhamID, ChiNhanhID, SoLo),
    CONSTRAINT CHK_LoHang_SoLuong CHECK (SoLuongConLai <= SoLuongNhap)
);
GO

-- =============================================
-- 5. NHÓM: KHUYẾN MÃI
-- =============================================

CREATE TABLE KhuyenMai (
    KhuyenMaiID INT IDENTITY(1,1) PRIMARY KEY,
    MaCode NVARCHAR(30) NOT NULL UNIQUE,
    MoTa NVARCHAR(255) NULL,
    LoaiGiam NVARCHAR(20) NOT NULL,
    GiaTriGiam DECIMAL(18,2) NOT NULL,
    GiaTriDonHangToiThieu DECIMAL(18,2) NOT NULL DEFAULT 0,
    NgayBatDau DATETIME NOT NULL,
    NgayKetThuc DATETIME NOT NULL,
    SoLuotSuDungToiDa INT NOT NULL DEFAULT 1,
    SoLuotDaSuDung INT NOT NULL DEFAULT 0,
    TrangThai BIT NOT NULL DEFAULT 1,
    CHECK (NgayKetThuc >= NgayBatDau)
);
GO

-- =============================================
-- 6. NHÓM: ĐƠN HÀNG
-- =============================================

CREATE TABLE TrangThaiDonHang (
    TrangThaiID INT IDENTITY(1,1) PRIMARY KEY,
    TenTrangThai NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE DonHang (
    DonHangID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    ChiNhanhID INT NULL,                     -- Chi nhánh xử lý đơn hàng
    TrangThaiID INT NOT NULL,
    KhuyenMaiID INT NULL,
    DiaChiGiaoHang NVARCHAR(255) NOT NULL,
    SoDienThoaiNhan NVARCHAR(15) NOT NULL,
    PhuongThucThanhToan NVARCHAR(30) NOT NULL,
    TienHang DECIMAL(18,2) NOT NULL,
    TienGiamGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    TongTien DECIMAL(18,2) NOT NULL,
    GhiChu NVARCHAR(255) NULL,
    NgayDatHang DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DonHang_KhachHang FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID),
    CONSTRAINT FK_DonHang_ChiNhanh FOREIGN KEY (ChiNhanhID) REFERENCES ChiNhanh(ChiNhanhID),
    CONSTRAINT FK_DonHang_TrangThai FOREIGN KEY (TrangThaiID) REFERENCES TrangThaiDonHang(TrangThaiID),
    CONSTRAINT FK_DonHang_KhuyenMai FOREIGN KEY (KhuyenMaiID) REFERENCES KhuyenMai(KhuyenMaiID)
);
GO

CREATE TABLE ChiTietDonHang (
    ChiTietDonHangID INT IDENTITY(1,1) PRIMARY KEY,
    DonHangID INT NOT NULL,
    SanPhamID INT NOT NULL,
    LoHangID INT NULL,                       -- Lô cụ thể đã xuất kho cho dòng đơn hàng này (phục vụ truy vết đổi trả)
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien AS (SoLuong * DonGia) PERSISTED,
    CONSTRAINT FK_CTDH_DonHang FOREIGN KEY (DonHangID) REFERENCES DonHang(DonHangID) ON DELETE CASCADE,
    CONSTRAINT FK_CTDH_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID),
    CONSTRAINT FK_CTDH_LoHang FOREIGN KEY (LoHangID) REFERENCES LoHang(LoHangID)
);
GO

CREATE TABLE LichSuTrangThaiDonHang (
    LichSuID INT IDENTITY(1,1) PRIMARY KEY,
    DonHangID INT NOT NULL,
    TrangThaiID INT NOT NULL,
    ThoiGianCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    NhanVienID INT NULL,
    CONSTRAINT FK_LSTT_DonHang FOREIGN KEY (DonHangID) REFERENCES DonHang(DonHangID) ON DELETE CASCADE,
    CONSTRAINT FK_LSTT_TrangThai FOREIGN KEY (TrangThaiID) REFERENCES TrangThaiDonHang(TrangThaiID),
    CONSTRAINT FK_LSTT_NhanVien FOREIGN KEY (NhanVienID) REFERENCES NhanVien(NhanVienID)
);
GO

-- =============================================
-- 7. NHÓM: PHIẾU ĐIỀU CHUYỂN KHO
-- =============================================

CREATE TABLE PhieuDieuChuyenKho (
    PhieuDieuChuyenID INT IDENTITY(1,1) PRIMARY KEY,
    ChiNhanhGuiID INT NOT NULL,
    ChiNhanhNhanID INT NOT NULL,
    NhanVienTaoID INT NOT NULL,
    NhanVienXacNhanID INT NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ChoXacNhan',  -- ChoXacNhan, DaXacNhan, DaHuy
    GhiChu NVARCHAR(255) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayXacNhan DATETIME NULL,
    CONSTRAINT FK_PDCK_ChiNhanhGui FOREIGN KEY (ChiNhanhGuiID) REFERENCES ChiNhanh(ChiNhanhID),
    CONSTRAINT FK_PDCK_ChiNhanhNhan FOREIGN KEY (ChiNhanhNhanID) REFERENCES ChiNhanh(ChiNhanhID),
    CONSTRAINT FK_PDCK_NhanVienTao FOREIGN KEY (NhanVienTaoID) REFERENCES NhanVien(NhanVienID),
    CONSTRAINT FK_PDCK_NhanVienXacNhan FOREIGN KEY (NhanVienXacNhanID) REFERENCES NhanVien(NhanVienID),
    CONSTRAINT CHK_PDCK_KhacChiNhanh CHECK (ChiNhanhGuiID <> ChiNhanhNhanID)
);
GO

CREATE TABLE ChiTietPhieuDieuChuyen (
    ChiTietID INT IDENTITY(1,1) PRIMARY KEY,
    PhieuDieuChuyenID INT NOT NULL,
    LoHangID INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    CONSTRAINT FK_CTPDC_Phieu FOREIGN KEY (PhieuDieuChuyenID) REFERENCES PhieuDieuChuyenKho(PhieuDieuChuyenID) ON DELETE CASCADE,
    CONSTRAINT FK_CTPDC_LoHang FOREIGN KEY (LoHangID) REFERENCES LoHang(LoHangID)
);
GO

-- =============================================
-- 8. NHÓM: ĐÁNH GIÁ SẢN PHẨM
-- =============================================

CREATE TABLE DanhGia (
    DanhGiaID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    KhachHangID INT NOT NULL,
    DonHangID INT NOT NULL,
    SoSao INT NOT NULL CHECK (SoSao BETWEEN 1 AND 5),
    NoiDung NVARCHAR(500) NULL,
    NgayDanhGia DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DanhGia_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID),
    CONSTRAINT FK_DanhGia_KhachHang FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID),
    CONSTRAINT FK_DanhGia_DonHang FOREIGN KEY (DonHangID) REFERENCES DonHang(DonHangID)
);
GO

-- =============================================
-- 9. NHÓM: GIỎ HÀNG
-- =============================================

CREATE TABLE GioHang (
    GioHangID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL UNIQUE,
    CONSTRAINT FK_GioHang_KhachHang FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID)
);
GO

CREATE TABLE ChiTietGioHang (
    ChiTietGioHangID INT IDENTITY(1,1) PRIMARY KEY,
    GioHangID INT NOT NULL,
    SanPhamID INT NOT NULL,
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    NgayThem DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CTGH_GioHang FOREIGN KEY (GioHangID) REFERENCES GioHang(GioHangID) ON DELETE CASCADE,
    CONSTRAINT FK_CTGH_SanPham FOREIGN KEY (SanPhamID) REFERENCES SanPham(SanPhamID)
);
GO

-- =============================================
-- 10. NHÓM: CHATBOT
-- =============================================

CREATE TABLE CuocHoiThoai (
    CuocHoiThoaiID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    NgayBatDau DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangMo',
    CONSTRAINT FK_CHT_KhachHang FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID)
);
GO

CREATE TABLE TinNhan (
    TinNhanID INT IDENTITY(1,1) PRIMARY KEY,
    CuocHoiThoaiID INT NOT NULL,
    NguoiGui NVARCHAR(20) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    ThoiGianGui DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TinNhan_CuocHoiThoai FOREIGN KEY (CuocHoiThoaiID) REFERENCES CuocHoiThoai(CuocHoiThoaiID) ON DELETE CASCADE
);
GO

-- =============================================
-- 11. NHÓM: THÔNG BÁO
-- =============================================

CREATE TABLE ThongBao (
    ThongBaoID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    TieuDe NVARCHAR(150) NOT NULL,
    NoiDung NVARCHAR(500) NULL,
    DaDoc BIT NOT NULL DEFAULT 0,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ThongBao_KhachHang FOREIGN KEY (KhachHangID) REFERENCES KhachHang(KhachHangID)
);
GO

-- =============================================
-- 12. DỮ LIỆU MẪU: HỆ THỐNG
-- =============================================

INSERT INTO VaiTro (TenVaiTro) VALUES (N'Admin'), (N'NhanVien'), (N'KhachHang');
GO

INSERT INTO TrangThaiDonHang (TenTrangThai) VALUES
(N'ChoXacNhan'), (N'DangXuLy'), (N'DangGiao'), (N'DaGiao'), (N'HoanThanh'), (N'DaHuy');
GO

-- =============================================
-- DỮ LIỆU MẪU: CHI NHÁNH
-- =============================================
INSERT INTO ChiNhanh (TenChiNhanh, DiaChi, SoDienThoai, LaTruSoChinh, TrangThai) VALUES
(N'Chi nhánh Quận 1', N'123 Nguyễn Huệ, Quận 1, TP.HCM', N'0281234567', 1, 1),
(N'Chi nhánh Thủ Đức', N'45 Võ Văn Ngân, Thủ Đức, TP.HCM', N'0287654321', 0, 1);
GO

-- =============================================
-- DỮ LIỆU MẪU: DANH MỤC
-- =============================================
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES
(N'Trà xanh', N'Các loại trà xanh sấy khô truyền thống'),
(N'Trà hoa', N'Trà ướp hoa tự nhiên như hoa nhài, hoa cúc'),
(N'Trà thảo mộc', N'Trà từ thảo dược, không chứa caffeine'),
(N'Trà ô long', N'Trà ô long lên men bán phần'),
(N'Trà đen', N'Trà đen lên men hoàn toàn, vị đậm');
GO

-- =============================================
-- DỮ LIỆU MẪU: THÀNH PHẦN
-- =============================================
INSERT INTO ThanhPhan (TenThanhPhan, MoTa) VALUES
(N'Lá trà xanh', N'Lá trà tươi được sấy khô tự nhiên'),
(N'Hoa cúc', N'Hoa cúc khô nguyên bông'),
(N'Hoa nhài', N'Hoa nhài ướp hương'),
(N'Cam thảo', N'Rễ cam thảo khô'),
(N'Gừng', N'Gừng khô thái lát'),
(N'Atiso', N'Bông atiso sấy khô'),
(N'Bạc hà', N'Lá bạc hà khô'),
(N'Kỷ tử', N'Quả kỷ tử khô');
GO

-- =============================================
-- DỮ LIỆU MẪU: CÔNG DỤNG
-- =============================================
INSERT INTO CongDung (TenCongDung, MoTa) VALUES
(N'Thanh nhiệt', N'Giúp giải nhiệt cơ thể'),
(N'An thần, dễ ngủ', N'Hỗ trợ thư giãn, cải thiện giấc ngủ'),
(N'Giảm cân', N'Hỗ trợ quá trình trao đổi chất'),
(N'Giải độc gan', N'Hỗ trợ chức năng gan'),
(N'Đẹp da', N'Chống oxy hóa, hỗ trợ làn da'),
(N'Tăng cường tiêu hóa', N'Hỗ trợ hệ tiêu hóa hoạt động tốt'),
(N'Giảm căng thẳng', N'Giúp thư giãn tinh thần'),
(N'Chống oxy hóa', N'Giàu chất chống oxy hóa tự nhiên');
GO

-- =============================================
-- DỮ LIỆU MẪU: SẢN PHẨM (SoLuongTon/HanSuDung đặt tạm, sẽ tự đồng bộ từ LoHang qua code)
-- =============================================
INSERT INTO SanPham (TenSanPham, DanhMucID, MoTaChiTiet, XuatXu, GiaBan, SoLuongTon, DonViTinh, HanSuDung, TrangThai)
VALUES
(N'Trà xanh Thái Nguyên', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà xanh'),
 N'Trà xanh nguyên chất hái từ vùng chè Thái Nguyên, sấy khô thủ công.', N'Thái Nguyên, Việt Nam', 85000, 0, N'Gói 100g', NULL, N'DangBan'),

(N'Trà hoa cúc mật ong', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà hoa'),
 N'Hoa cúc khô nguyên bông kết hợp mật ong, vị thanh dịu.', N'Đà Lạt, Việt Nam', 95000, 0, N'Hộp 50 túi lọc', NULL, N'DangBan'),

(N'Trà atiso Đà Lạt', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà thảo mộc'),
 N'Atiso sấy khô nguyên bông, hỗ trợ giải độc gan.', N'Đà Lạt, Việt Nam', 70000, 0, N'Gói 200g', NULL, N'DangBan'),

(N'Trà ô long sữa Đài Loan', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà ô long'),
 N'Trà ô long hương sữa tự nhiên, nhập khẩu Đài Loan.', N'Đài Loan', 150000, 0, N'Hộp 100g', NULL, N'DangBan'),

(N'Trà gừng mật ong', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà thảo mộc'),
 N'Gừng khô kết hợp mật ong, làm ấm cơ thể.', N'Việt Nam', 65000, 0, N'Hộp 20 túi lọc', NULL, N'DangBan'),

(N'Trà nhài thượng hạng', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà hoa'),
 N'Trà xanh ướp hoa nhài tự nhiên nhiều lần.', N'Việt Nam', 110000, 0, N'Gói 150g', NULL, N'DangBan'),

(N'Trà đen Ceylon', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà đen'),
 N'Trà đen nhập khẩu từ Sri Lanka, vị đậm đà.', N'Sri Lanka', 130000, 0, N'Hộp 100g', NULL, N'DangBan'),

(N'Trà bạc hà thanh lọc', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà thảo mộc'),
 N'Lá bạc hà khô nguyên chất, giúp thư giãn.', N'Việt Nam', 60000, 0, N'Gói 100g', NULL, N'DangBan'),

(N'Trà kỷ tử táo đỏ', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà thảo mộc'),
 N'Kỷ tử kết hợp táo đỏ, bồi bổ cơ thể.', N'Việt Nam', 120000, 0, N'Hộp 20 túi lọc', NULL, N'DangBan'),

(N'Trà xanh matcha sấy lạnh', (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà xanh'),
 N'Trà xanh sấy lạnh giữ trọn hương vị và dưỡng chất.', N'Việt Nam', 140000, 0, N'Gói 100g', NULL, N'HetHang');
GO

-- =============================================
-- DỮ LIỆU MẪU: SẢN PHẨM - THÀNH PHẦN
-- =============================================
INSERT INTO SanPham_ThanhPhan (SanPhamID, ThanhPhanID)
SELECT sp.SanPhamID, tp.ThanhPhanID FROM SanPham sp, ThanhPhan tp
WHERE (sp.TenSanPham = N'Trà xanh Thái Nguyên' AND tp.TenThanhPhan = N'Lá trà xanh')
   OR (sp.TenSanPham = N'Trà hoa cúc mật ong' AND tp.TenThanhPhan = N'Hoa cúc')
   OR (sp.TenSanPham = N'Trà atiso Đà Lạt' AND tp.TenThanhPhan = N'Atiso')
   OR (sp.TenSanPham = N'Trà gừng mật ong' AND tp.TenThanhPhan = N'Gừng')
   OR (sp.TenSanPham = N'Trà nhài thượng hạng' AND tp.TenThanhPhan = N'Hoa nhài')
   OR (sp.TenSanPham = N'Trà nhài thượng hạng' AND tp.TenThanhPhan = N'Lá trà xanh')
   OR (sp.TenSanPham = N'Trà bạc hà thanh lọc' AND tp.TenThanhPhan = N'Bạc hà')
   OR (sp.TenSanPham = N'Trà kỷ tử táo đỏ' AND tp.TenThanhPhan = N'Kỷ tử')
   OR (sp.TenSanPham = N'Trà xanh matcha sấy lạnh' AND tp.TenThanhPhan = N'Lá trà xanh');
GO

-- =============================================
-- DỮ LIỆU MẪU: SẢN PHẨM - CÔNG DỤNG
-- =============================================
INSERT INTO SanPham_CongDung (SanPhamID, CongDungID)
SELECT sp.SanPhamID, cd.CongDungID FROM SanPham sp, CongDung cd
WHERE (sp.TenSanPham = N'Trà xanh Thái Nguyên' AND cd.TenCongDung = N'Chống oxy hóa')
   OR (sp.TenSanPham = N'Trà xanh Thái Nguyên' AND cd.TenCongDung = N'Giảm cân')
   OR (sp.TenSanPham = N'Trà hoa cúc mật ong' AND cd.TenCongDung = N'An thần, dễ ngủ')
   OR (sp.TenSanPham = N'Trà hoa cúc mật ong' AND cd.TenCongDung = N'Thanh nhiệt')
   OR (sp.TenSanPham = N'Trà atiso Đà Lạt' AND cd.TenCongDung = N'Giải độc gan')
   OR (sp.TenSanPham = N'Trà gừng mật ong' AND cd.TenCongDung = N'Tăng cường tiêu hóa')
   OR (sp.TenSanPham = N'Trà nhài thượng hạng' AND cd.TenCongDung = N'Giảm căng thẳng')
   OR (sp.TenSanPham = N'Trà bạc hà thanh lọc' AND cd.TenCongDung = N'Giảm căng thẳng')
   OR (sp.TenSanPham = N'Trà kỷ tử táo đỏ' AND cd.TenCongDung = N'Đẹp da')
   OR (sp.TenSanPham = N'Trà xanh matcha sấy lạnh' AND cd.TenCongDung = N'Chống oxy hóa');
GO

-- =============================================
-- DỮ LIỆU MẪU: HÌNH ẢNH SẢN PHẨM
-- =============================================
INSERT INTO HinhAnhSanPham (SanPhamID, DuongDanAnh, ThuTuHienThi)
SELECT SanPhamID, N'/images/products/' + CAST(SanPhamID AS NVARCHAR) + N'_1.jpg', 1 FROM SanPham;
GO

-- =============================================
-- DỮ LIỆU MẪU: LÔ HÀNG (thay thế cho SoLuongTon cố định trước đây)
-- =============================================
DECLARE @CN1 INT = (SELECT ChiNhanhID FROM ChiNhanh WHERE LaTruSoChinh = 1);
DECLARE @CN2 INT = (SELECT ChiNhanhID FROM ChiNhanh WHERE LaTruSoChinh = 0);

-- Trà xanh Thái Nguyên: 2 lô ở 2 chi nhánh khác nhau
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN1, N'LOT-001', '2026-01-15', '2027-06-30', 70, 70 FROM SanPham WHERE TenSanPham = N'Trà xanh Thái Nguyên';
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN2, N'LOT-002', '2026-02-01', '2027-08-15', 50, 50 FROM SanPham WHERE TenSanPham = N'Trà xanh Thái Nguyên';

-- Trà hoa cúc mật ong
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN1, N'LOT-001', '2026-01-10', '2027-03-15', 80, 80 FROM SanPham WHERE TenSanPham = N'Trà hoa cúc mật ong';

-- Trà atiso Đà Lạt
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN1, N'LOT-001', '2025-12-20', '2027-01-20', 150, 150 FROM SanPham WHERE TenSanPham = N'Trà atiso Đà Lạt';

-- Trà ô long sữa Đài Loan
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN2, N'LOT-001', '2026-01-05', '2026-12-10', 60, 60 FROM SanPham WHERE TenSanPham = N'Trà ô long sữa Đài Loan';

-- Trà gừng mật ong
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN1, N'LOT-001', '2026-01-20', '2027-02-28', 100, 100 FROM SanPham WHERE TenSanPham = N'Trà gừng mật ong';

-- Trà nhài thượng hạng
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN2, N'LOT-001', '2026-02-10', '2027-05-01', 70, 70 FROM SanPham WHERE TenSanPham = N'Trà nhài thượng hạng';

-- Trà đen Ceylon
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN1, N'LOT-001', '2025-11-15', '2026-11-15', 45, 45 FROM SanPham WHERE TenSanPham = N'Trà đen Ceylon';

-- Trà bạc hà thanh lọc
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN1, N'LOT-001', '2026-02-05', '2027-04-10', 90, 90 FROM SanPham WHERE TenSanPham = N'Trà bạc hà thanh lọc';

-- Trà kỷ tử táo đỏ
INSERT INTO LoHang (SanPhamID, ChiNhanhID, SoLo, NgayNhap, HanSuDung, SoLuongNhap, SoLuongConLai)
SELECT SanPhamID, @CN2, N'LOT-001', '2026-02-15', '2027-07-01', 55, 55 FROM SanPham WHERE TenSanPham = N'Trà kỷ tử táo đỏ';

-- Trà xanh matcha sấy lạnh: hết hàng, không có lô nào còn hàng
GO

-- =============================================
-- ĐỒNG BỘ TẠM SoLuongTon/HanSuDung TỪ LoHang (sau này code C# tự làm việc này khi có thay đổi)
-- =============================================
UPDATE sp
SET sp.SoLuongTon = ISNULL(tong.TongConLai, 0),
    sp.HanSuDung = hsd.HanGanNhat
FROM SanPham sp
OUTER APPLY (
    SELECT SUM(SoLuongConLai) AS TongConLai FROM LoHang WHERE SanPhamID = sp.SanPhamID AND TrangThai = N'ConHang'
) tong
OUTER APPLY (
    SELECT MIN(HanSuDung) AS HanGanNhat FROM LoHang WHERE SanPhamID = sp.SanPhamID AND TrangThai = N'ConHang'
) hsd;
GO

-- =============================================
-- DỮ LIỆU MẪU: TÀI KHOẢN
-- =============================================
INSERT INTO TaiKhoan (TenDangNhap, MatKhauHash, Email, SoDienThoai, VaiTroID, TrangThai) VALUES
(N'admin', N'$2a$hashed_password_admin', N'admin@trasaykho.vn', N'0901000001',
 (SELECT VaiTroID FROM VaiTro WHERE TenVaiTro = N'Admin'), 1),
(N'nhanvien01', N'$2a$hashed_password_nv01', N'nhanvien01@trasaykho.vn', N'0901000002',
 (SELECT VaiTroID FROM VaiTro WHERE TenVaiTro = N'NhanVien'), 1),
(N'nhanvien02', N'$2a$hashed_password_nv02', N'nhanvien02@trasaykho.vn', N'0901000005',
 (SELECT VaiTroID FROM VaiTro WHERE TenVaiTro = N'NhanVien'), 1),
(N'khachhang01', N'$2a$hashed_password_kh01', N'khachhang01@gmail.com', N'0901000003',
 (SELECT VaiTroID FROM VaiTro WHERE TenVaiTro = N'KhachHang'), 1),
(N'khachhang02', N'$2a$hashed_password_kh02', N'khachhang02@gmail.com', N'0901000004',
 (SELECT VaiTroID FROM VaiTro WHERE TenVaiTro = N'KhachHang'), 1);
GO

-- Admin Khải: ChiNhanhID = NULL → quản trị viên tổng
INSERT INTO NhanVien (TaiKhoanID, ChiNhanhID, HoTen, ChucVu, NgayVaoLam)
SELECT TaiKhoanID, NULL, N'Nguyễn Văn Khải', N'Quản trị viên tổng', '2025-01-01'
FROM TaiKhoan WHERE TenDangNhap = N'admin';

-- Nhân viên 01: thuộc Chi nhánh Quận 1
INSERT INTO NhanVien (TaiKhoanID, ChiNhanhID, HoTen, ChucVu, NgayVaoLam)
SELECT TaiKhoanID, (SELECT ChiNhanhID FROM ChiNhanh WHERE LaTruSoChinh = 1), N'Phan Thanh Bình', N'Quản lý chi nhánh', '2025-02-01'
FROM TaiKhoan WHERE TenDangNhap = N'nhanvien01';

-- Nhân viên 02: thuộc Chi nhánh Thủ Đức
INSERT INTO NhanVien (TaiKhoanID, ChiNhanhID, HoTen, ChucVu, NgayVaoLam)
SELECT TaiKhoanID, (SELECT ChiNhanhID FROM ChiNhanh WHERE LaTruSoChinh = 0), N'Trần Minh Phúc', N'Quản lý chi nhánh', '2025-03-01'
FROM TaiKhoan WHERE TenDangNhap = N'nhanvien02';
GO

INSERT INTO KhachHang (TaiKhoanID, HoTen, NgaySinh, GioiTinh, DiaChi)
SELECT TaiKhoanID, N'Trần Thị Mai', '1998-05-10', N'Nữ', N'123 Nguyễn Trãi, Quận 1, TP.HCM'
FROM TaiKhoan WHERE TenDangNhap = N'khachhang01';

INSERT INTO KhachHang (TaiKhoanID, HoTen, NgaySinh, GioiTinh, DiaChi)
SELECT TaiKhoanID, N'Lê Văn Nam', '1995-11-22', N'Nam', N'45 Lê Lợi, Quận 3, TP.HCM'
FROM TaiKhoan WHERE TenDangNhap = N'khachhang02';
GO

-- =============================================
-- DỮ LIỆU MẪU: KHUYẾN MÃI
-- =============================================
INSERT INTO KhuyenMai (MaCode, MoTa, LoaiGiam, GiaTriGiam, GiaTriDonHangToiThieu, NgayBatDau, NgayKetThuc, SoLuotSuDungToiDa, TrangThai)
VALUES
(N'TRAMOI10', N'Giảm 10% cho đơn hàng đầu tiên', N'PhanTram', 10, 100000, '2026-08-01', '2026-12-31', 500, 1),
(N'FREESHIP', N'Giảm 30.000đ phí vận chuyển', N'SoTien', 30000, 200000, '2026-08-01', '2026-09-30', 200, 1);
GO

-- =============================================
-- DỮ LIỆU MẪU: 1 ĐƠN HÀNG TEST (gắn Chi nhánh + Lô hàng cụ thể)
-- =============================================
DECLARE @KhachHangID INT = (SELECT KhachHangID FROM KhachHang kh
    JOIN TaiKhoan tk ON kh.TaiKhoanID = tk.TaiKhoanID WHERE tk.TenDangNhap = N'khachhang01');
DECLARE @ChiNhanhDon INT = (SELECT ChiNhanhID FROM ChiNhanh WHERE LaTruSoChinh = 1);
DECLARE @TrangThaiDaGiao INT = (SELECT TrangThaiID FROM TrangThaiDonHang WHERE TenTrangThai = N'DaGiao');
DECLARE @SanPham1 INT = (SELECT SanPhamID FROM SanPham WHERE TenSanPham = N'Trà xanh Thái Nguyên');
DECLARE @SanPham2 INT = (SELECT SanPhamID FROM SanPham WHERE TenSanPham = N'Trà hoa cúc mật ong');
DECLARE @LoHang1 INT = (SELECT LoHangID FROM LoHang WHERE SanPhamID = @SanPham1 AND ChiNhanhID = @ChiNhanhDon);
DECLARE @LoHang2 INT = (SELECT LoHangID FROM LoHang WHERE SanPhamID = @SanPham2 AND ChiNhanhID = @ChiNhanhDon);

INSERT INTO DonHang (KhachHangID, ChiNhanhID, TrangThaiID, DiaChiGiaoHang, SoDienThoaiNhan, PhuongThucThanhToan, TienHang, TienGiamGia, TongTien, NgayDatHang)
VALUES (@KhachHangID, @ChiNhanhDon, @TrangThaiDaGiao, N'123 Nguyễn Trãi, Quận 1, TP.HCM', N'0901000003', N'COD', 265000, 0, 265000, '2026-08-05');

DECLARE @DonHangID INT = SCOPE_IDENTITY();

INSERT INTO ChiTietDonHang (DonHangID, SanPhamID, LoHangID, SoLuong, DonGia) VALUES
(@DonHangID, @SanPham1, @LoHang1, 2, 85000),
(@DonHangID, @SanPham2, @LoHang2, 1, 95000);

INSERT INTO LichSuTrangThaiDonHang (DonHangID, TrangThaiID) VALUES
(@DonHangID, (SELECT TrangThaiID FROM TrangThaiDonHang WHERE TenTrangThai = N'ChoXacNhan')),
(@DonHangID, (SELECT TrangThaiID FROM TrangThaiDonHang WHERE TenTrangThai = N'DangXuLy')),
(@DonHangID, (SELECT TrangThaiID FROM TrangThaiDonHang WHERE TenTrangThai = N'DangGiao')),
(@DonHangID, @TrangThaiDaGiao);

INSERT INTO DanhGia (SanPhamID, KhachHangID, DonHangID, SoSao, NoiDung) VALUES
(@SanPham1, @KhachHangID, @DonHangID, 5, N'Trà rất thơm, đóng gói cẩn thận, sẽ ủng hộ tiếp!');
GO