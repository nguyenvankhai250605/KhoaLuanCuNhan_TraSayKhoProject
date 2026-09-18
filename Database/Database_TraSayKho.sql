USE master;
GO

IF DB_ID(N'TraSayKhoDB') IS NOT NULL
BEGIN
    ALTER DATABASE TraSayKhoDB
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE TraSayKhoDB;
END
GO

CREATE DATABASE TraSayKhoDB;
GO

USE TraSayKhoDB;
GO

/* =========================================================
   1. CHI NHÁNH
   ========================================================= */

CREATE TABLE ChiNhanh
(
    ChiNhanhID INT IDENTITY(1,1) PRIMARY KEY,
    TenChiNhanh NVARCHAR(150) NOT NULL,
    DiaChi NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(15) NULL,
    LaTruSoChinh BIT NOT NULL DEFAULT 0,
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_ChiNhanh_Ten UNIQUE (TenChiNhanh)
);
GO

-- Toàn hệ thống chỉ được có một trụ sở chính
CREATE UNIQUE INDEX UX_ChiNhanh_ChiMotTruSoChinh
ON ChiNhanh(LaTruSoChinh)
WHERE LaTruSoChinh = 1;
GO

/* =========================================================
   2. TÀI KHOẢN VÀ PHÂN QUYỀN
   ========================================================= */

CREATE TABLE VaiTro
(
    VaiTroID INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL,

    CONSTRAINT UQ_VaiTro_TenVaiTro UNIQUE (TenVaiTro)
);
GO

CREATE TABLE TaiKhoan
(
    TaiKhoanID INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL,
    MatKhauHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(15) NULL,
    VaiTroID INT NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_TaiKhoan_TenDangNhap UNIQUE (TenDangNhap),
    CONSTRAINT UQ_TaiKhoan_Email UNIQUE (Email),

    CONSTRAINT FK_TaiKhoan_VaiTro
        FOREIGN KEY (VaiTroID)
        REFERENCES VaiTro(VaiTroID)
);
GO

CREATE TABLE KhachHang
(
    KhachHangID INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanID INT NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(10) NULL,
    DiaChi NVARCHAR(255) NULL,
    AvatarURL NVARCHAR(255) NULL,

    CONSTRAINT UQ_KhachHang_TaiKhoan UNIQUE (TaiKhoanID),

    CONSTRAINT CHK_KhachHang_GioiTinh
        CHECK (GioiTinh IS NULL OR GioiTinh IN (N'Nam', N'Nữ', N'Khác')),

    CONSTRAINT FK_KhachHang_TaiKhoan
        FOREIGN KEY (TaiKhoanID)
        REFERENCES TaiKhoan(TaiKhoanID)
);
GO

CREATE TABLE NhanVien
(
    NhanVienID INT IDENTITY(1,1) PRIMARY KEY,
    TaiKhoanID INT NOT NULL,
    ChiNhanhID INT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NULL,
    NgayVaoLam DATE NULL,

    CONSTRAINT UQ_NhanVien_TaiKhoan UNIQUE (TaiKhoanID),

    CONSTRAINT FK_NhanVien_TaiKhoan
        FOREIGN KEY (TaiKhoanID)
        REFERENCES TaiKhoan(TaiKhoanID),

    CONSTRAINT FK_NhanVien_ChiNhanh
        FOREIGN KEY (ChiNhanhID)
        REFERENCES ChiNhanh(ChiNhanhID)
);
GO

/* =========================================================
   3. DANH MỤC, THÀNH PHẦN, CÔNG DỤNG VÀ SẢN PHẨM
   ========================================================= */

CREATE TABLE DanhMuc
(
    DanhMucID INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    DanhMucChaID INT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_DanhMuc_Ten UNIQUE (TenDanhMuc),

    CONSTRAINT FK_DanhMuc_DanhMucCha
        FOREIGN KEY (DanhMucChaID)
        REFERENCES DanhMuc(DanhMucID)
);
GO

CREATE TABLE ThanhPhan
(
    ThanhPhanID INT IDENTITY(1,1) PRIMARY KEY,
    TenThanhPhan NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,

    CONSTRAINT UQ_ThanhPhan_Ten UNIQUE (TenThanhPhan)
);
GO

CREATE TABLE CongDung
(
    CongDungID INT IDENTITY(1,1) PRIMARY KEY,
    TenCongDung NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,

    CONSTRAINT UQ_CongDung_Ten UNIQUE (TenCongDung)
);
GO

CREATE TABLE SanPham
(
    SanPhamID INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(150) NOT NULL,
    DanhMucID INT NOT NULL,
    MoTaChiTiet NVARCHAR(MAX) NULL,
    XuatXu NVARCHAR(100) NULL,
    GiaBan DECIMAL(18,2) NOT NULL,
    SoLuongTon INT NOT NULL DEFAULT 0,
    DonViTinh NVARCHAR(20) NOT NULL,
    KhoiLuongGam DECIMAL(10,2) NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangBan',
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CHK_SanPham_GiaBan
        CHECK (GiaBan >= 0),

    CONSTRAINT CHK_SanPham_SoLuongTon
        CHECK (SoLuongTon >= 0),

    CONSTRAINT CHK_SanPham_KhoiLuong
        CHECK (KhoiLuongGam IS NULL OR KhoiLuongGam > 0),

    CONSTRAINT CHK_SanPham_DonViTinh
        CHECK (DonViTinh IN (N'Hộp', N'Gói')),

    CONSTRAINT CHK_SanPham_TrangThai
        CHECK (TrangThai IN (N'DangBan', N'NgungBan', N'HetHang')),

    CONSTRAINT FK_SanPham_DanhMuc
        FOREIGN KEY (DanhMucID)
        REFERENCES DanhMuc(DanhMucID)
);
GO

CREATE TABLE SanPham_ThanhPhan
(
    SanPhamID INT NOT NULL,
    ThanhPhanID INT NOT NULL,

    CONSTRAINT PK_SanPham_ThanhPhan
        PRIMARY KEY (SanPhamID, ThanhPhanID),

    CONSTRAINT FK_SPTP_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
        ON DELETE CASCADE,

    CONSTRAINT FK_SPTP_ThanhPhan
        FOREIGN KEY (ThanhPhanID)
        REFERENCES ThanhPhan(ThanhPhanID)
);
GO

CREATE TABLE SanPham_CongDung
(
    SanPhamID INT NOT NULL,
    CongDungID INT NOT NULL,

    CONSTRAINT PK_SanPham_CongDung
        PRIMARY KEY (SanPhamID, CongDungID),

    CONSTRAINT FK_SPCD_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
        ON DELETE CASCADE,

    CONSTRAINT FK_SPCD_CongDung
        FOREIGN KEY (CongDungID)
        REFERENCES CongDung(CongDungID)
);
GO

CREATE TABLE HinhAnhSanPham
(
    HinhAnhID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    DuongDanAnh NVARCHAR(255) NOT NULL,
    ThuTuHienThi INT NOT NULL DEFAULT 0,

    CONSTRAINT CHK_HinhAnh_ThuTu
        CHECK (ThuTuHienThi >= 0),

    CONSTRAINT UQ_HinhAnh_SanPham_ThuTu
        UNIQUE (SanPhamID, ThuTuHienThi),

    CONSTRAINT FK_HinhAnh_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
        ON DELETE CASCADE
);
GO

/* =========================================================
   4. LÔ HÀNG, THÙNG HÀNG VÀ ĐƠN VỊ SẢN PHẨM
   ========================================================= */

CREATE TABLE LoHang
(
    LoHangID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    SoLo NVARCHAR(50) NOT NULL,
    NgaySanXuat DATE NOT NULL,
    HanSuDung DATE NOT NULL,
    TongSoLuongNhap INT NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ConHang',
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_LoHang_SoLo UNIQUE (SoLo),

    CONSTRAINT CHK_LoHang_SoLuongNhap
        CHECK (TongSoLuongNhap > 0),

    CONSTRAINT CHK_LoHang_HanSuDung
        CHECK (HanSuDung > NgaySanXuat),

    CONSTRAINT CHK_LoHang_TrangThai
        CHECK (TrangThai IN (N'ConHang', N'HetHang', N'HetHan', N'DaHuy')),

    CONSTRAINT FK_LoHang_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
);
GO

CREATE TABLE ThungHang
(
    ThungID INT IDENTITY(1,1) PRIMARY KEY,
    LoHangID INT NOT NULL,
    ChiNhanhID INT NOT NULL,
    MaThung NVARCHAR(50) NOT NULL,
    SoLuongDonVi INT NOT NULL,
    NgayPhanBo DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ConHang',

    CONSTRAINT UQ_ThungHang_MaThung UNIQUE (MaThung),

    CONSTRAINT CHK_ThungHang_SoLuongDonVi
        CHECK (SoLuongDonVi > 0),

    CONSTRAINT CHK_ThungHang_TrangThai
        CHECK (TrangThai IN (
            N'ConHang',
            N'HetHang',
            N'DangVanChuyen',
            N'DaHuy'
        )),

    CONSTRAINT FK_ThungHang_LoHang
        FOREIGN KEY (LoHangID)
        REFERENCES LoHang(LoHangID),

    CONSTRAINT FK_ThungHang_ChiNhanh
        FOREIGN KEY (ChiNhanhID)
        REFERENCES ChiNhanh(ChiNhanhID)
);
GO

-- Mỗi dòng là một đơn vị bán vật lý hoàn chỉnh.
-- Đơn vị có thể là Hộp hoặc Gói, được xác định qua:
-- DonViSanPham -> ThungHang -> LoHang -> SanPham -> DonViTinh.
CREATE TABLE DonViSanPham
(
    DonViID INT IDENTITY(1,1) PRIMARY KEY,
    ThungID INT NOT NULL,
    MaDonVi NVARCHAR(50) NOT NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ConKho',
    ChiTietDonHangID INT NULL,
    NgayBan DATETIME NULL,

    CONSTRAINT UQ_DonViSanPham_MaDonVi UNIQUE (MaDonVi),

    CONSTRAINT CHK_DonViSanPham_TrangThai
        CHECK (TrangThai IN (N'ConKho', N'DaBan', N'DaHuy')),

    CONSTRAINT CHK_DonViSanPham_ThongTinBan
        CHECK (
            (TrangThai = N'DaBan'
                AND ChiTietDonHangID IS NOT NULL
                AND NgayBan IS NOT NULL)
            OR
            (TrangThai <> N'DaBan'
                AND ChiTietDonHangID IS NULL
                AND NgayBan IS NULL)
        ),

    CONSTRAINT FK_DonViSanPham_ThungHang
        FOREIGN KEY (ThungID)
        REFERENCES ThungHang(ThungID)
);
GO

/* =========================================================
   5. BẬC GIẢM GIÁ XẢ KHO
   ========================================================= */

CREATE TABLE BacGiamGiaXaKho
(
    BacGiamGiaID INT IDENTITY(1,1) PRIMARY KEY,
    DanhMucID INT NULL,
    TenBac NVARCHAR(50) NOT NULL,
    PhanTramThoiGianConLaiToiDa DECIMAL(5,2) NOT NULL,
    MucGiamGiaPhanTram DECIMAL(5,2) NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,

    CONSTRAINT CHK_BacGiamGia_ThoiGian
        CHECK (
            PhanTramThoiGianConLaiToiDa > 0
            AND PhanTramThoiGianConLaiToiDa <= 100
        ),

    CONSTRAINT CHK_BacGiamGia_MucGiam
        CHECK (
            MucGiamGiaPhanTram > 0
            AND MucGiamGiaPhanTram <= 100
        ),

    CONSTRAINT FK_BacGiamGia_DanhMuc
        FOREIGN KEY (DanhMucID)
        REFERENCES DanhMuc(DanhMucID)
);
GO

-- Không cho trùng ngưỡng giảm giá riêng trong cùng danh mục
CREATE UNIQUE INDEX UX_BacGiamGia_DanhMuc_Nguong
ON BacGiamGiaXaKho(DanhMucID, PhanTramThoiGianConLaiToiDa)
WHERE DanhMucID IS NOT NULL;
GO

-- Không cho trùng ngưỡng giảm giá dùng chung
CREATE UNIQUE INDEX UX_BacGiamGia_Chung_Nguong
ON BacGiamGiaXaKho(PhanTramThoiGianConLaiToiDa)
WHERE DanhMucID IS NULL;
GO

/* =========================================================
   6. HẠN MỨC SẢN PHẨM
   ========================================================= */

CREATE TABLE HanMucSanPham
(
    HanMucID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    ChiNhanhID INT NULL,
    HanMucBanNgay INT NULL,
    HanMucBanTuan INT NULL,
    TonKhoToiThieu INT NULL,
    TonKhoToiDa INT NULL,
    SoNgayLuuKhoToiDa INT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,

    CONSTRAINT CHK_HanMuc_BanNgay
        CHECK (HanMucBanNgay IS NULL OR HanMucBanNgay > 0),

    CONSTRAINT CHK_HanMuc_BanTuan
        CHECK (HanMucBanTuan IS NULL OR HanMucBanTuan > 0),

    CONSTRAINT CHK_HanMuc_TonKhoToiThieu
        CHECK (TonKhoToiThieu IS NULL OR TonKhoToiThieu >= 0),

    CONSTRAINT CHK_HanMuc_TonKhoToiDa
        CHECK (TonKhoToiDa IS NULL OR TonKhoToiDa >= 0),

    CONSTRAINT CHK_HanMuc_TonKho
        CHECK (
            TonKhoToiThieu IS NULL
            OR TonKhoToiDa IS NULL
            OR TonKhoToiDa >= TonKhoToiThieu
        ),

    CONSTRAINT CHK_HanMuc_SoNgayLuuKho
        CHECK (SoNgayLuuKhoToiDa IS NULL OR SoNgayLuuKhoToiDa > 0),

    CONSTRAINT FK_HanMuc_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID),

    CONSTRAINT FK_HanMuc_ChiNhanh
        FOREIGN KEY (ChiNhanhID)
        REFERENCES ChiNhanh(ChiNhanhID)
);
GO

-- Hạn mức riêng theo chi nhánh
CREATE UNIQUE INDEX UX_HanMuc_SanPham_ChiNhanh
ON HanMucSanPham(SanPhamID, ChiNhanhID)
WHERE ChiNhanhID IS NOT NULL;
GO

-- Mỗi sản phẩm chỉ có một hạn mức chung
CREATE UNIQUE INDEX UX_HanMuc_SanPham_Chung
ON HanMucSanPham(SanPhamID)
WHERE ChiNhanhID IS NULL;
GO

/* =========================================================
   7. KHUYẾN MÃI
   ========================================================= */

CREATE TABLE KhuyenMai
(
    KhuyenMaiID INT IDENTITY(1,1) PRIMARY KEY,
    MaCode NVARCHAR(30) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    LoaiGiam NVARCHAR(20) NOT NULL,
    GiaTriGiam DECIMAL(18,2) NOT NULL,
    GiaTriDonHangToiThieu DECIMAL(18,2) NOT NULL DEFAULT 0,
    NgayBatDau DATETIME NOT NULL,
    NgayKetThuc DATETIME NOT NULL,
    SoLuotSuDungToiDa INT NOT NULL DEFAULT 1,
    SoLuotDaSuDung INT NOT NULL DEFAULT 0,
    TrangThai BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_KhuyenMai_MaCode UNIQUE (MaCode),

    CONSTRAINT CHK_KhuyenMai_LoaiGiam
        CHECK (LoaiGiam IN (N'PhanTram', N'SoTien')),

    CONSTRAINT CHK_KhuyenMai_GiaTriGiam
        CHECK (
            GiaTriGiam > 0
            AND (
                LoaiGiam <> N'PhanTram'
                OR GiaTriGiam <= 100
            )
        ),

    CONSTRAINT CHK_KhuyenMai_DonHangToiThieu
        CHECK (GiaTriDonHangToiThieu >= 0),

    CONSTRAINT CHK_KhuyenMai_ThoiGian
        CHECK (NgayKetThuc >= NgayBatDau),

    CONSTRAINT CHK_KhuyenMai_SoLuot
        CHECK (
            SoLuotSuDungToiDa > 0
            AND SoLuotDaSuDung >= 0
            AND SoLuotDaSuDung <= SoLuotSuDungToiDa
        )
);
GO

/* =========================================================
   8. ĐƠN HÀNG
   ========================================================= */

CREATE TABLE TrangThaiDonHang
(
    TrangThaiID INT IDENTITY(1,1) PRIMARY KEY,
    TenTrangThai NVARCHAR(50) NOT NULL,

    CONSTRAINT UQ_TrangThaiDonHang_Ten UNIQUE (TenTrangThai)
);
GO

CREATE TABLE DonHang
(
    DonHangID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    ChiNhanhID INT NULL,
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

    CONSTRAINT CHK_DonHang_TienHang
        CHECK (TienHang >= 0),

    CONSTRAINT CHK_DonHang_TienGiam
        CHECK (TienGiamGia >= 0 AND TienGiamGia <= TienHang),

    CONSTRAINT CHK_DonHang_TongTien
        CHECK (TongTien >= 0),

    CONSTRAINT CHK_DonHang_ThanhTien
        CHECK (TongTien = TienHang - TienGiamGia),

    CONSTRAINT CHK_DonHang_PhuongThucThanhToan
        CHECK (PhuongThucThanhToan IN (
            N'TienMat',
            N'ChuyenKhoan',
            N'COD',
            N'VNPay',
            N'MoMo'
        )),

    CONSTRAINT FK_DonHang_KhachHang
        FOREIGN KEY (KhachHangID)
        REFERENCES KhachHang(KhachHangID),

    CONSTRAINT FK_DonHang_ChiNhanh
        FOREIGN KEY (ChiNhanhID)
        REFERENCES ChiNhanh(ChiNhanhID),

    CONSTRAINT FK_DonHang_TrangThai
        FOREIGN KEY (TrangThaiID)
        REFERENCES TrangThaiDonHang(TrangThaiID),

    CONSTRAINT FK_DonHang_KhuyenMai
        FOREIGN KEY (KhuyenMaiID)
        REFERENCES KhuyenMai(KhuyenMaiID)
);
GO

CREATE TABLE ChiTietDonHang
(
    ChiTietDonHangID INT IDENTITY(1,1) PRIMARY KEY,
    DonHangID INT NOT NULL,
    SanPhamID INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien AS
        (CONVERT(DECIMAL(18,2), SoLuong * DonGia)) PERSISTED,

    CONSTRAINT CHK_ChiTietDonHang_SoLuong
        CHECK (SoLuong > 0),

    CONSTRAINT CHK_ChiTietDonHang_DonGia
        CHECK (DonGia >= 0),

    CONSTRAINT FK_CTDH_DonHang
        FOREIGN KEY (DonHangID)
        REFERENCES DonHang(DonHangID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CTDH_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
);
GO

ALTER TABLE DonViSanPham
ADD CONSTRAINT FK_DonViSanPham_ChiTietDonHang
    FOREIGN KEY (ChiTietDonHangID)
    REFERENCES ChiTietDonHang(ChiTietDonHangID);
GO

CREATE TABLE LichSuTrangThaiDonHang
(
    LichSuID INT IDENTITY(1,1) PRIMARY KEY,
    DonHangID INT NOT NULL,
    TrangThaiID INT NOT NULL,
    ThoiGianCapNhat DATETIME NOT NULL DEFAULT GETDATE(),
    NhanVienID INT NULL,

    CONSTRAINT FK_LSTT_DonHang
        FOREIGN KEY (DonHangID)
        REFERENCES DonHang(DonHangID)
        ON DELETE CASCADE,

    CONSTRAINT FK_LSTT_TrangThai
        FOREIGN KEY (TrangThaiID)
        REFERENCES TrangThaiDonHang(TrangThaiID),

    CONSTRAINT FK_LSTT_NhanVien
        FOREIGN KEY (NhanVienID)
        REFERENCES NhanVien(NhanVienID)
);
GO

/* =========================================================
   9. ĐIỀU CHUYỂN KHO
   ========================================================= */

CREATE TABLE PhieuDieuChuyenKho
(
    PhieuDieuChuyenID INT IDENTITY(1,1) PRIMARY KEY,
    ChiNhanhGuiID INT NOT NULL,
    ChiNhanhNhanID INT NOT NULL,
    NhanVienTaoID INT NOT NULL,
    NhanVienXacNhanID INT NULL,
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'ChoDuyet',
    GhiChu NVARCHAR(255) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayXacNhan DATETIME NULL,

    CONSTRAINT CHK_PDCK_KhacChiNhanh
        CHECK (ChiNhanhGuiID <> ChiNhanhNhanID),

    CONSTRAINT CHK_PDCK_TrangThai
        CHECK (TrangThai IN (
            N'ChoDuyet',
            N'DaDuyet',
            N'TuChoi',
            N'DangVanChuyen',
            N'HoanTat',
            N'DaHuy'
        )),

    CONSTRAINT FK_PDCK_ChiNhanhGui
        FOREIGN KEY (ChiNhanhGuiID)
        REFERENCES ChiNhanh(ChiNhanhID),

    CONSTRAINT FK_PDCK_ChiNhanhNhan
        FOREIGN KEY (ChiNhanhNhanID)
        REFERENCES ChiNhanh(ChiNhanhID),

    CONSTRAINT FK_PDCK_NhanVienTao
        FOREIGN KEY (NhanVienTaoID)
        REFERENCES NhanVien(NhanVienID),

    CONSTRAINT FK_PDCK_NhanVienXacNhan
        FOREIGN KEY (NhanVienXacNhanID)
        REFERENCES NhanVien(NhanVienID)
);
GO

CREATE TABLE ChiTietPhieuDieuChuyen
(
    ChiTietID INT IDENTITY(1,1) PRIMARY KEY,
    PhieuDieuChuyenID INT NOT NULL,
    ThungID INT NOT NULL,

    CONSTRAINT UQ_CTPDC_Phieu_Thung
        UNIQUE (PhieuDieuChuyenID, ThungID),

    CONSTRAINT FK_CTPDC_Phieu
        FOREIGN KEY (PhieuDieuChuyenID)
        REFERENCES PhieuDieuChuyenKho(PhieuDieuChuyenID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CTPDC_ThungHang
        FOREIGN KEY (ThungID)
        REFERENCES ThungHang(ThungID)
);
GO

/* =========================================================
   10. ĐÁNH GIÁ
   ========================================================= */

CREATE TABLE DanhGia
(
    DanhGiaID INT IDENTITY(1,1) PRIMARY KEY,
    SanPhamID INT NOT NULL,
    KhachHangID INT NOT NULL,
    DonHangID INT NOT NULL,
    SoSao INT NOT NULL,
    NoiDung NVARCHAR(500) NULL,
    NgayDanhGia DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_DanhGia_DonHang_SanPham
        UNIQUE (DonHangID, SanPhamID),

    CONSTRAINT CHK_DanhGia_SoSao
        CHECK (SoSao BETWEEN 1 AND 5),

    CONSTRAINT FK_DanhGia_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID),

    CONSTRAINT FK_DanhGia_KhachHang
        FOREIGN KEY (KhachHangID)
        REFERENCES KhachHang(KhachHangID),

    CONSTRAINT FK_DanhGia_DonHang
        FOREIGN KEY (DonHangID)
        REFERENCES DonHang(DonHangID)
);
GO

/* =========================================================
   11. GIỎ HÀNG
   ========================================================= */

CREATE TABLE GioHang
(
    GioHangID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,

    CONSTRAINT UQ_GioHang_KhachHang UNIQUE (KhachHangID),

    CONSTRAINT FK_GioHang_KhachHang
        FOREIGN KEY (KhachHangID)
        REFERENCES KhachHang(KhachHangID)
);
GO

CREATE TABLE ChiTietGioHang
(
    ChiTietGioHangID INT IDENTITY(1,1) PRIMARY KEY,
    GioHangID INT NOT NULL,
    SanPhamID INT NOT NULL,
    SoLuong INT NOT NULL,
    NgayThem DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_ChiTietGioHang_GioHang_SanPham
        UNIQUE (GioHangID, SanPhamID),

    CONSTRAINT CHK_ChiTietGioHang_SoLuong
        CHECK (SoLuong > 0),

    CONSTRAINT FK_CTGH_GioHang
        FOREIGN KEY (GioHangID)
        REFERENCES GioHang(GioHangID)
        ON DELETE CASCADE,

    CONSTRAINT FK_CTGH_SanPham
        FOREIGN KEY (SanPhamID)
        REFERENCES SanPham(SanPhamID)
);
GO

/* =========================================================
   12. CHATBOT
   ========================================================= */

CREATE TABLE CuocHoiThoai
(
    CuocHoiThoaiID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    NgayBatDau DATETIME NOT NULL DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'DangMo',

    CONSTRAINT CHK_CuocHoiThoai_TrangThai
        CHECK (TrangThai IN (N'DangMo', N'DaDong')),

    CONSTRAINT FK_CHT_KhachHang
        FOREIGN KEY (KhachHangID)
        REFERENCES KhachHang(KhachHangID)
);
GO

CREATE TABLE TinNhan
(
    TinNhanID INT IDENTITY(1,1) PRIMARY KEY,
    CuocHoiThoaiID INT NOT NULL,
    NguoiGui NVARCHAR(20) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    ThoiGianGui DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CHK_TinNhan_NguoiGui
        CHECK (NguoiGui IN (N'KhachHang', N'Chatbot')),

    CONSTRAINT FK_TinNhan_CuocHoiThoai
        FOREIGN KEY (CuocHoiThoaiID)
        REFERENCES CuocHoiThoai(CuocHoiThoaiID)
        ON DELETE CASCADE
);
GO

/* =========================================================
   13. THÔNG BÁO
   ========================================================= */

CREATE TABLE ThongBao
(
    ThongBaoID INT IDENTITY(1,1) PRIMARY KEY,
    KhachHangID INT NOT NULL,
    TieuDe NVARCHAR(150) NOT NULL,
    NoiDung NVARCHAR(500) NULL,
    DaDoc BIT NOT NULL DEFAULT 0,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ThongBao_KhachHang
        FOREIGN KEY (KhachHangID)
        REFERENCES KhachHang(KhachHangID)
        ON DELETE CASCADE
);
GO

/* =========================================================
   14. INDEX HỖ TRỢ TRA CỨU
   ========================================================= */

CREATE INDEX IX_TaiKhoan_VaiTroID
ON TaiKhoan(VaiTroID);
GO

CREATE INDEX IX_NhanVien_ChiNhanhID
ON NhanVien(ChiNhanhID);
GO

CREATE INDEX IX_SanPham_DanhMucID_TrangThai
ON SanPham(DanhMucID, TrangThai);
GO

CREATE INDEX IX_LoHang_SanPhamID_HanSuDung
ON LoHang(SanPhamID, HanSuDung);
GO

CREATE INDEX IX_ThungHang_ChiNhanhID_TrangThai
ON ThungHang(ChiNhanhID, TrangThai);
GO

CREATE INDEX IX_DonViSanPham_ThungID_TrangThai
ON DonViSanPham(ThungID, TrangThai);
GO

CREATE INDEX IX_DonHang_KhachHangID_NgayDatHang
ON DonHang(KhachHangID, NgayDatHang DESC);
GO

CREATE INDEX IX_DonHang_ChiNhanhID_TrangThaiID
ON DonHang(ChiNhanhID, TrangThaiID);
GO

CREATE INDEX IX_LichSuTrangThai_DonHangID
ON LichSuTrangThaiDonHang(DonHangID, ThoiGianCapNhat);
GO

CREATE INDEX IX_PhieuDieuChuyen_ChiNhanhGui
ON PhieuDieuChuyenKho(ChiNhanhGuiID, TrangThai);
GO

CREATE INDEX IX_PhieuDieuChuyen_ChiNhanhNhan
ON PhieuDieuChuyenKho(ChiNhanhNhanID, TrangThai);
GO

CREATE INDEX IX_ThongBao_KhachHangID_DaDoc
ON ThongBao(KhachHangID, DaDoc, NgayTao DESC);
GO

/* =========================================================
   15. DỮ LIỆU NỀN
   ========================================================= */

INSERT INTO VaiTro(TenVaiTro)
VALUES
    (N'Admin'),
    (N'NhanVien'),
    (N'KhachHang'),
    (N'ChuCuaHang');
GO

INSERT INTO TrangThaiDonHang(TenTrangThai)
VALUES
    (N'ChoXacNhan'),
    (N'DangXuLy'),
    (N'DangGiao'),
    (N'DaGiao'),
    (N'HoanThanh'),
    (N'DaHuy');
GO

INSERT INTO ChiNhanh
(
    TenChiNhanh,
    DiaChi,
    SoDienThoai,
    LaTruSoChinh,
    TrangThai
)
VALUES
(
    N'Chi nhánh Quận 1',
    N'123 Nguyễn Huệ, Quận 1, TP.HCM',
    N'0281234567',
    1,
    1
),
(
    N'Chi nhánh Thủ Đức',
    N'45 Võ Văn Ngân, Thủ Đức, TP.HCM',
    N'0287654321',
    0,
    1
);
GO

INSERT INTO BacGiamGiaXaKho
(
    DanhMucID,
    TenBac,
    PhanTramThoiGianConLaiToiDa,
    MucGiamGiaPhanTram,
    TrangThai
)
VALUES
    (NULL, N'Đợt 1 - Cận hạn', 25, 15, 1),
    (NULL, N'Đợt 2 - Gần hết hạn', 10, 30, 1),
    (NULL, N'Đợt 3 - Sắp hết hạn', 3, 50, 1);
GO

INSERT INTO DanhMuc(TenDanhMuc, MoTa)
VALUES
    (N'Trà xanh', N'Các loại trà xanh sấy khô truyền thống'),
    (N'Trà hoa', N'Trà ướp hoa tự nhiên'),
    (N'Trà thảo mộc', N'Trà từ thảo dược'),
    (N'Trà ô long', N'Trà ô long lên men bán phần'),
    (N'Trà đen', N'Trà đen lên men hoàn toàn');
GO

INSERT INTO ThanhPhan(TenThanhPhan, MoTa)
VALUES
    (N'Lá trà xanh', N'Lá trà tươi được sấy khô'),
    (N'Hoa cúc', N'Hoa cúc khô nguyên bông'),
    (N'Hoa nhài', N'Hoa nhài ướp hương'),
    (N'Cam thảo', N'Rễ cam thảo khô'),
    (N'Gừng', N'Gừng khô thái lát'),
    (N'Atiso', N'Bông atiso sấy khô'),
    (N'Bạc hà', N'Lá bạc hà khô'),
    (N'Kỷ tử', N'Quả kỷ tử khô');
GO

INSERT INTO CongDung(TenCongDung, MoTa)
VALUES
    (N'Thanh nhiệt', N'Giúp giải nhiệt cơ thể'),
    (N'An thần, dễ ngủ', N'Hỗ trợ cải thiện giấc ngủ'),
    (N'Giảm cân', N'Hỗ trợ quá trình trao đổi chất'),
    (N'Giải độc gan', N'Hỗ trợ chức năng gan'),
    (N'Đẹp da', N'Hỗ trợ làn da'),
    (N'Tăng cường tiêu hóa', N'Hỗ trợ hệ tiêu hóa'),
    (N'Giảm căng thẳng', N'Giúp thư giãn tinh thần'),
    (N'Chống oxy hóa', N'Giàu chất chống oxy hóa');
GO

/* =========================================================
   16. DỮ LIỆU SẢN PHẨM MẪU
   SanPham không còn cột HanSuDung
   ========================================================= */

INSERT INTO SanPham
(
    TenSanPham,
    DanhMucID,
    MoTaChiTiet,
    XuatXu,
    GiaBan,
    SoLuongTon,
    DonViTinh,
    KhoiLuongGam,
    TrangThai
)
VALUES
(
    N'Trà xanh Thái Nguyên gói 100g',
    (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà xanh'),
    N'Trà xanh nguyên chất được sấy khô thủ công.',
    N'Thái Nguyên, Việt Nam',
    85000,
    0,
    N'Gói',
    100,
    N'DangBan'
),
(
    N'Trà hoa cúc mật ong hộp 75g',
    (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà hoa'),
    N'Hoa cúc khô nguyên bông kết hợp mật ong.',
    N'Đà Lạt, Việt Nam',
    95000,
    0,
    N'Hộp',
    75,
    N'DangBan'
),
(
    N'Trà atiso Đà Lạt gói 200g',
    (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà thảo mộc'),
    N'Atiso sấy khô hỗ trợ giải độc gan.',
    N'Đà Lạt, Việt Nam',
    70000,
    0,
    N'Gói',
    200,
    N'DangBan'
),
(
    N'Trà ô long sữa Đài Loan hộp 100g',
    (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà ô long'),
    N'Trà ô long hương sữa tự nhiên.',
    N'Đài Loan',
    150000,
    0,
    N'Hộp',
    100,
    N'DangBan'
),
(
    N'Trà gừng mật ong hộp 60g',
    (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà thảo mộc'),
    N'Gừng khô kết hợp mật ong.',
    N'Việt Nam',
    65000,
    0,
    N'Hộp',
    60,
    N'DangBan'
),
(
    N'Trà xanh Thái Nguyên hộp 200g',
    (SELECT DanhMucID FROM DanhMuc WHERE TenDanhMuc = N'Trà xanh'),
    N'Cùng dòng trà xanh Thái Nguyên nhưng được đóng theo quy cách hộp 200g.',
    N'Thái Nguyên, Việt Nam',
    155000,
    0,
    N'Hộp',
    200,
    N'DangBan'
);
GO

INSERT INTO SanPham_ThanhPhan(SanPhamID, ThanhPhanID)
SELECT sp.SanPhamID, tp.ThanhPhanID
FROM SanPham sp
JOIN ThanhPhan tp
    ON
       (sp.TenSanPham IN (N'Trà xanh Thái Nguyên gói 100g', N'Trà xanh Thái Nguyên hộp 200g')
        AND tp.TenThanhPhan = N'Lá trà xanh')
    OR (sp.TenSanPham = N'Trà hoa cúc mật ong hộp 75g'
        AND tp.TenThanhPhan = N'Hoa cúc')
    OR (sp.TenSanPham = N'Trà atiso Đà Lạt gói 200g'
        AND tp.TenThanhPhan = N'Atiso')
    OR (sp.TenSanPham = N'Trà gừng mật ong hộp 60g'
        AND tp.TenThanhPhan = N'Gừng');
GO

INSERT INTO SanPham_CongDung(SanPhamID, CongDungID)
SELECT sp.SanPhamID, cd.CongDungID
FROM SanPham sp
JOIN CongDung cd
    ON
       (sp.TenSanPham IN (N'Trà xanh Thái Nguyên gói 100g', N'Trà xanh Thái Nguyên hộp 200g')
        AND cd.TenCongDung = N'Chống oxy hóa')
    OR (sp.TenSanPham = N'Trà hoa cúc mật ong hộp 75g'
        AND cd.TenCongDung = N'An thần, dễ ngủ')
    OR (sp.TenSanPham = N'Trà atiso Đà Lạt gói 200g'
        AND cd.TenCongDung = N'Giải độc gan')
    OR (sp.TenSanPham = N'Trà gừng mật ong hộp 60g'
        AND cd.TenCongDung = N'Tăng cường tiêu hóa');
GO

INSERT INTO HinhAnhSanPham
(
    SanPhamID,
    DuongDanAnh,
    ThuTuHienThi
)
SELECT
    SanPhamID,
    N'/images/products/' + CAST(SanPhamID AS NVARCHAR(10)) + N'_1.jpg',
    1
FROM SanPham;
GO

/* =========================================================
   17. LÔ, THÙNG VÀ ĐƠN VỊ SẢN PHẨM MẪU

   Quy ước nghiệp vụ:
   - Hộp và Gói là hai quy cách bán độc lập.
   - Mỗi quy cách là một dòng SanPham riêng.
   - Mỗi đơn vị vật lý có MaDonVi duy nhất để truy xuất.
   - Ví dụ lô gói: 100 thùng, phân bổ 70 thùng cho CN1
     và 30 thùng cho CN2; mỗi thùng chứa 10 gói.
   ========================================================= */

DECLARE @CN1 INT =
(
    SELECT ChiNhanhID
    FROM ChiNhanh
    WHERE LaTruSoChinh = 1
);

DECLARE @CN2 INT =
(
    SELECT TOP 1 ChiNhanhID
    FROM ChiNhanh
    WHERE LaTruSoChinh = 0
    ORDER BY ChiNhanhID
);

DECLARE @SPGoi INT =
(
    SELECT SanPhamID
    FROM SanPham
    WHERE TenSanPham = N'Trà xanh Thái Nguyên gói 100g'
);

DECLARE @SPHop INT =
(
    SELECT SanPhamID
    FROM SanPham
    WHERE TenSanPham = N'Trà xanh Thái Nguyên hộp 200g'
);

-- =========================================================
-- Lô 1: quy cách Gói, tổng 1.000 gói
-- 100 thùng x 10 gói; phân bổ 70 thùng / 30 thùng
-- =========================================================
INSERT INTO LoHang
(
    SanPhamID,
    SoLo,
    NgaySanXuat,
    HanSuDung,
    TongSoLuongNhap,
    TrangThai
)
VALUES
(
    @SPGoi,
    N'LO-GOI-TN-20260115',
    '2026-01-15',
    '2027-06-30',
    1000,
    N'ConHang'
);

DECLARE @LoGoi INT = SCOPE_IDENTITY();
DECLARE @SoThung INT = 1;
DECLARE @SoDonVi INT = 1;
DECLARE @ThungMoiID INT;
DECLARE @j INT;

WHILE @SoThung <= 100
BEGIN
    INSERT INTO ThungHang
    (
        LoHangID,
        ChiNhanhID,
        MaThung,
        SoLuongDonVi,
        TrangThai
    )
    VALUES
    (
        @LoGoi,
        CASE WHEN @SoThung <= 70 THEN @CN1 ELSE @CN2 END,
        N'THUNG-GOI-' + RIGHT(N'000' + CAST(@SoThung AS NVARCHAR(10)), 3),
        10,
        N'ConHang'
    );

    SET @ThungMoiID = SCOPE_IDENTITY();
    SET @j = 1;

    WHILE @j <= 10
    BEGIN
        INSERT INTO DonViSanPham(ThungID, MaDonVi, TrangThai)
        VALUES
        (
            @ThungMoiID,
            N'GOI-TN-' + RIGHT(N'000000' + CAST(@SoDonVi AS NVARCHAR(10)), 6),
            N'ConKho'
        );

        SET @SoDonVi += 1;
        SET @j += 1;
    END

    SET @SoThung += 1;
END

-- =========================================================
-- Lô 2: quy cách Hộp, tổng 50 hộp
-- 10 thùng x 5 hộp; phân bổ 7 thùng / 3 thùng
-- =========================================================
INSERT INTO LoHang
(
    SanPhamID,
    SoLo,
    NgaySanXuat,
    HanSuDung,
    TongSoLuongNhap,
    TrangThai
)
VALUES
(
    @SPHop,
    N'LO-HOP-TN-20260201',
    '2026-02-01',
    '2027-08-31',
    50,
    N'ConHang'
);

DECLARE @LoHop INT = SCOPE_IDENTITY();
SET @SoThung = 1;
SET @SoDonVi = 1;

WHILE @SoThung <= 10
BEGIN
    INSERT INTO ThungHang
    (
        LoHangID,
        ChiNhanhID,
        MaThung,
        SoLuongDonVi,
        TrangThai
    )
    VALUES
    (
        @LoHop,
        CASE WHEN @SoThung <= 7 THEN @CN1 ELSE @CN2 END,
        N'THUNG-HOP-' + RIGHT(N'000' + CAST(@SoThung AS NVARCHAR(10)), 3),
        5,
        N'ConHang'
    );

    SET @ThungMoiID = SCOPE_IDENTITY();
    SET @j = 1;

    WHILE @j <= 5
    BEGIN
        INSERT INTO DonViSanPham(ThungID, MaDonVi, TrangThai)
        VALUES
        (
            @ThungMoiID,
            N'HOP-TN-' + RIGHT(N'000000' + CAST(@SoDonVi AS NVARCHAR(10)), 6),
            N'ConKho'
        );

        SET @SoDonVi += 1;
        SET @j += 1;
    END

    SET @SoThung += 1;
END
GO

-- Đồng bộ tồn kho từ các đơn vị vật lý còn trong kho.
-- SanPham.SoLuongTon được tính theo đúng DonViTinh của từng sản phẩm.
UPDATE sp
SET SoLuongTon =
(
    SELECT COUNT(*)
    FROM DonViSanPham dv
    JOIN ThungHang th
        ON th.ThungID = dv.ThungID
    JOIN LoHang lh
        ON lh.LoHangID = th.LoHangID
    WHERE lh.SanPhamID = sp.SanPhamID
      AND dv.TrangThai = N'ConKho'
)
FROM SanPham sp;
GO

INSERT INTO HanMucSanPham
(
    SanPhamID,
    ChiNhanhID,
    HanMucBanNgay,
    HanMucBanTuan,
    TonKhoToiThieu,
    TonKhoToiDa,
    SoNgayLuuKhoToiDa,
    TrangThai
)
VALUES
(
    (SELECT SanPhamID FROM SanPham
     WHERE TenSanPham = N'Trà xanh Thái Nguyên gói 100g'),
    NULL,
    100,
    500,
    200,
    1200,
    540,
    1
),
(
    (SELECT SanPhamID FROM SanPham
     WHERE TenSanPham = N'Trà xanh Thái Nguyên hộp 200g'),
    NULL,
    20,
    100,
    10,
    100,
    540,
    1
);
GO

/* =========================================================
   18. TÀI KHOẢN MẪU
   Mật khẩu Admin: Admin@123
   ========================================================= */

INSERT INTO TaiKhoan
(
    TenDangNhap,
    MatKhauHash,
    Email,
    SoDienThoai,
    VaiTroID,
    TrangThai
)
VALUES
(
    N'admin',
    N'$2b$12$NpQr484holn3x9KcF9cYieGqaP87fUSRBiEdFWxcHzGItsggxL3d6',
    N'admin@trasaykho.vn',
    N'0901000001',
    (SELECT VaiTroID FROM VaiTro WHERE TenVaiTro = N'Admin'),
    1
);
GO

INSERT INTO NhanVien
(
    TaiKhoanID,
    ChiNhanhID,
    HoTen,
    ChucVu,
    NgayVaoLam
)
SELECT
    TaiKhoanID,
    NULL,
    N'Nguyễn Văn Khải',
    N'Quản trị viên hệ thống',
    '2025-01-01'
FROM TaiKhoan
WHERE TenDangNhap = N'admin';
GO

/* =========================================================
   19. KHUYẾN MÃI MẪU
   ========================================================= */

INSERT INTO KhuyenMai
(
    MaCode,
    MoTa,
    LoaiGiam,
    GiaTriGiam,
    GiaTriDonHangToiThieu,
    NgayBatDau,
    NgayKetThuc,
    SoLuotSuDungToiDa,
    SoLuotDaSuDung,
    TrangThai
)
VALUES
(
    N'TRAMOI10',
    N'Giảm 10% cho đơn hàng đầu tiên',
    N'PhanTram',
    10,
    100000,
    '2026-01-01',
    '2027-12-31',
    500,
    0,
    1
),
(
    N'GIAM30K',
    N'Giảm 30.000 đồng cho đơn hàng từ 200.000 đồng',
    N'SoTien',
    30000,
    200000,
    '2026-01-01',
    '2027-12-31',
    200,
    0,
    1
);
GO

/* =========================================================
   20. KIỂM TRA SAU KHI TẠO DATABASE
   ========================================================= */

SELECT * FROM VaiTro;
SELECT * FROM ChiNhanh;
SELECT * FROM DanhMuc;
SELECT * FROM SanPham;
SELECT * FROM LoHang;
SELECT * FROM ThungHang;
SELECT TOP 10 * FROM DonViSanPham;
SELECT * FROM TaiKhoan;
SELECT * FROM NhanVien;

-- Kiểm tra tồn kho riêng theo quy cách Hộp/Gói
SELECT
    sp.SanPhamID,
    sp.TenSanPham,
    sp.DonViTinh,
    COUNT(dv.DonViID) AS SoDonViConKho
FROM SanPham sp
LEFT JOIN LoHang lh
    ON lh.SanPhamID = sp.SanPhamID
LEFT JOIN ThungHang th
    ON th.LoHangID = lh.LoHangID
LEFT JOIN DonViSanPham dv
    ON dv.ThungID = th.ThungID
   AND dv.TrangThai = N'ConKho'
GROUP BY
    sp.SanPhamID,
    sp.TenSanPham,
    sp.DonViTinh
ORDER BY sp.SanPhamID;

-- Kiểm tra lô Gói được phân bổ 70 thùng / 30 thùng
SELECT
    lh.SoLo,
    cn.TenChiNhanh,
    COUNT(DISTINCT th.ThungID) AS SoThung,
    COUNT(dv.DonViID) AS SoDonVi
FROM LoHang lh
JOIN ThungHang th
    ON th.LoHangID = lh.LoHangID
JOIN ChiNhanh cn
    ON cn.ChiNhanhID = th.ChiNhanhID
LEFT JOIN DonViSanPham dv
    ON dv.ThungID = th.ThungID
WHERE lh.SoLo = N'LO-GOI-TN-20260115'
GROUP BY
    lh.SoLo,
    cn.TenChiNhanh
ORDER BY cn.TenChiNhanh;
GO
