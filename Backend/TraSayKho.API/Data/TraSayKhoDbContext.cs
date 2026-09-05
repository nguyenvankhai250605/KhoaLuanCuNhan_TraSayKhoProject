using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Models;

namespace TraSayKho.API.Data;

public partial class TraSayKhoDbContext : DbContext
{
    public TraSayKhoDbContext(DbContextOptions<TraSayKhoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BacGiamGiaXaKho> BacGiamGiaXaKhos { get; set; }

    public virtual DbSet<ChiNhanh> ChiNhanhs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<ChiTietPhieuDieuChuyen> ChiTietPhieuDieuChuyens { get; set; }

    public virtual DbSet<CongDung> CongDungs { get; set; }

    public virtual DbSet<CuocHoiThoai> CuocHoiThoais { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; }

    public virtual DbSet<LoHang> LoHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuDieuChuyenKho> PhieuDieuChuyenKhos { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhPhan> ThanhPhans { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<TinNhan> TinNhans { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacGiamGiaXaKho>(entity =>
        {
            entity.HasKey(e => e.BacGiamGiaId).HasName("PK__BacGiamG__E6CEA39B1F3380A0");

            entity.ToTable("BacGiamGiaXaKho");

            entity.Property(e => e.BacGiamGiaId).HasColumnName("BacGiamGiaID");
            entity.Property(e => e.MucGiamGiaPhanTram).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenBac).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.ChiNhanhId).HasName("PK__ChiNhanh__0AC14C8E8BD5E680");

            entity.ToTable("ChiNhanh");

            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanhID");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenChiNhanh).HasMaxLength(150);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietDonHangId).HasName("PK__ChiTietD__45B33F834B3AF912");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.ChiTietDonHangId).HasColumnName("ChiTietDonHangID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.LoHangId).HasColumnName("LoHangID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("([SoLuong]*[DonGia])", true)
                .HasColumnType("decimal(29, 2)");

            entity.HasOne(d => d.DonHang).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK_CTDH_DonHang");

            entity.HasOne(d => d.LoHang).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.LoHangId)
                .HasConstraintName("FK_CTDH_LoHang");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDH_SanPham");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietGioHangId).HasName("PK__ChiTietG__EC01138DF61D2DB6");

            entity.ToTable("ChiTietGioHang");

            entity.Property(e => e.ChiTietGioHangId).HasColumnName("ChiTietGioHangID");
            entity.Property(e => e.GioHangId).HasColumnName("GioHangID");
            entity.Property(e => e.NgayThem)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

            entity.HasOne(d => d.GioHang).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.GioHangId)
                .HasConstraintName("FK_CTGH_GioHang");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTGH_SanPham");
        });

        modelBuilder.Entity<ChiTietPhieuDieuChuyen>(entity =>
        {
            entity.HasKey(e => e.ChiTietId).HasName("PK__ChiTietP__B117E9EAC284A162");

            entity.ToTable("ChiTietPhieuDieuChuyen");

            entity.Property(e => e.ChiTietId).HasColumnName("ChiTietID");
            entity.Property(e => e.LoHangId).HasColumnName("LoHangID");
            entity.Property(e => e.PhieuDieuChuyenId).HasColumnName("PhieuDieuChuyenID");

            entity.HasOne(d => d.LoHang).WithMany(p => p.ChiTietPhieuDieuChuyens)
                .HasForeignKey(d => d.LoHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPDC_LoHang");

            entity.HasOne(d => d.PhieuDieuChuyen).WithMany(p => p.ChiTietPhieuDieuChuyens)
                .HasForeignKey(d => d.PhieuDieuChuyenId)
                .HasConstraintName("FK_CTPDC_Phieu");
        });

        modelBuilder.Entity<CongDung>(entity =>
        {
            entity.HasKey(e => e.CongDungId).HasName("PK__CongDung__26D88B6A782AD3B4");

            entity.ToTable("CongDung");

            entity.HasIndex(e => e.TenCongDung, "UQ__CongDung__AC00B3624ACBCB36").IsUnique();

            entity.Property(e => e.CongDungId).HasColumnName("CongDungID");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenCongDung).HasMaxLength(100);
        });

        modelBuilder.Entity<CuocHoiThoai>(entity =>
        {
            entity.HasKey(e => e.CuocHoiThoaiId).HasName("PK__CuocHoiT__DB92E77E98AE491E");

            entity.ToTable("CuocHoiThoai");

            entity.Property(e => e.CuocHoiThoaiId).HasColumnName("CuocHoiThoaiID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("DangMo");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.CuocHoiThoais)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHT_KhachHang");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.DanhGiaId).HasName("PK__DanhGia__52C0CA2566194AF2");

            entity.Property(e => e.DanhGiaId).HasColumnName("DanhGiaID");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

            entity.HasOne(d => d.DonHang).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.DonHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGia_DonHang");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGia_KhachHang");

            entity.HasOne(d => d.SanPham).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGia_SanPham");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.DanhMucId).HasName("PK__DanhMuc__1C53BA7BA1B52EFB");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.DanhMucChaId).HasColumnName("DanhMucChaID");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.DanhMucCha).WithMany(p => p.InverseDanhMucCha)
                .HasForeignKey(d => d.DanhMucChaId)
                .HasConstraintName("FK_DanhMuc_DanhMucCha");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.DonHangId).HasName("PK__DonHang__D159F4DE4897C43F");

            entity.ToTable("DonHang");

            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanhID");
            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.KhuyenMaiId).HasColumnName("KhuyenMaiID");
            entity.Property(e => e.NgayDatHang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(30);
            entity.Property(e => e.SoDienThoaiNhan).HasMaxLength(15);
            entity.Property(e => e.TienGiamGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TienHang).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiId).HasColumnName("TrangThaiID");

            entity.HasOne(d => d.ChiNhanh).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.ChiNhanhId)
                .HasConstraintName("FK_DonHang_ChiNhanh");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_KhachHang");

            entity.HasOne(d => d.KhuyenMai).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.KhuyenMaiId)
                .HasConstraintName("FK_DonHang_KhuyenMai");

            entity.HasOne(d => d.TrangThai).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.TrangThaiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_TrangThai");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.GioHangId).HasName("PK__GioHang__4242280D030BE074");

            entity.ToTable("GioHang");

            entity.HasIndex(e => e.KhachHangId, "UQ__GioHang__880F211A222CC5FD").IsUnique();

            entity.Property(e => e.GioHangId).HasColumnName("GioHangID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");

            entity.HasOne(d => d.KhachHang).WithOne(p => p.GioHang)
                .HasForeignKey<GioHang>(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_KhachHang");
        });

        modelBuilder.Entity<HinhAnhSanPham>(entity =>
        {
            entity.HasKey(e => e.HinhAnhId).HasName("PK__HinhAnhS__8EF32B7BF5FCCC2B");

            entity.ToTable("HinhAnhSanPham");

            entity.Property(e => e.HinhAnhId).HasColumnName("HinhAnhID");
            entity.Property(e => e.DuongDanAnh).HasMaxLength(255);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

            entity.HasOne(d => d.SanPham).WithMany(p => p.HinhAnhSanPhams)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK_HinhAnh_SanPham");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211B5FBDB756");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.TaiKhoanId, "UQ__KhachHan__9A124B64EACBDDB5").IsUnique();

            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .HasColumnName("AvatarURL");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");

            entity.HasOne(d => d.TaiKhoan).WithOne(p => p.KhachHang)
                .HasForeignKey<KhachHang>(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhachHang_TaiKhoan");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.KhuyenMaiId).HasName("PK__KhuyenMa__820D74778CF2BF55");

            entity.ToTable("KhuyenMai");

            entity.HasIndex(e => e.MaCode, "UQ__KhuyenMa__152C7C5C7779BF0F").IsUnique();

            entity.Property(e => e.KhuyenMaiId).HasColumnName("KhuyenMaiID");
            entity.Property(e => e.GiaTriDonHangToiThieu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiGiam).HasMaxLength(20);
            entity.Property(e => e.MaCode).HasMaxLength(30);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.SoLuotSuDungToiDa).HasDefaultValue(1);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<LichSuTrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.LichSuId).HasName("PK__LichSuTr__CD0C1E3BFCF8E5E2");

            entity.ToTable("LichSuTrangThaiDonHang");

            entity.Property(e => e.LichSuId).HasColumnName("LichSuID");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.ThoiGianCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiId).HasColumnName("TrangThaiID");

            entity.HasOne(d => d.DonHang).WithMany(p => p.LichSuTrangThaiDonHangs)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK_LSTT_DonHang");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.LichSuTrangThaiDonHangs)
                .HasForeignKey(d => d.NhanVienId)
                .HasConstraintName("FK_LSTT_NhanVien");

            entity.HasOne(d => d.TrangThai).WithMany(p => p.LichSuTrangThaiDonHangs)
                .HasForeignKey(d => d.TrangThaiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LSTT_TrangThai");
        });

        modelBuilder.Entity<LoHang>(entity =>
        {
            entity.HasKey(e => e.LoHangId).HasName("PK__LoHang__AAA88484C7E2FB65");

            entity.ToTable("LoHang");

            entity.HasIndex(e => new { e.SanPhamId, e.ChiNhanhId, e.SoLo }, "UQ_LoHang_SoLo").IsUnique();

            entity.Property(e => e.LoHangId).HasColumnName("LoHangID");
            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanhID");
            entity.Property(e => e.MucGiamGiaHienTai).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.NgayNhap).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.SoLo).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ConHang");

            entity.HasOne(d => d.ChiNhanh).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.ChiNhanhId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoHang_ChiNhanh");

            entity.HasOne(d => d.SanPham).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoHang_SanPham");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EA3CD79B45");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.TaiKhoanId, "UQ__NhanVien__9A124B646602A465").IsUnique();

            entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");
            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanhID");
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");

            entity.HasOne(d => d.ChiNhanh).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.ChiNhanhId)
                .HasConstraintName("FK_NhanVien_ChiNhanh");

            entity.HasOne(d => d.TaiKhoan).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVien_TaiKhoan");
        });

        modelBuilder.Entity<PhieuDieuChuyenKho>(entity =>
        {
            entity.HasKey(e => e.PhieuDieuChuyenId).HasName("PK__PhieuDie__EE393D97AB573D36");

            entity.ToTable("PhieuDieuChuyenKho");

            entity.Property(e => e.PhieuDieuChuyenId).HasColumnName("PhieuDieuChuyenID");
            entity.Property(e => e.ChiNhanhGuiId).HasColumnName("ChiNhanhGuiID");
            entity.Property(e => e.ChiNhanhNhanId).HasColumnName("ChiNhanhNhanID");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayXacNhan).HasColumnType("datetime");
            entity.Property(e => e.NhanVienTaoId).HasColumnName("NhanVienTaoID");
            entity.Property(e => e.NhanVienXacNhanId).HasColumnName("NhanVienXacNhanID");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ChoXacNhan");

            entity.HasOne(d => d.ChiNhanhGui).WithMany(p => p.PhieuDieuChuyenKhoChiNhanhGuis)
                .HasForeignKey(d => d.ChiNhanhGuiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PDCK_ChiNhanhGui");

            entity.HasOne(d => d.ChiNhanhNhan).WithMany(p => p.PhieuDieuChuyenKhoChiNhanhNhans)
                .HasForeignKey(d => d.ChiNhanhNhanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PDCK_ChiNhanhNhan");

            entity.HasOne(d => d.NhanVienTao).WithMany(p => p.PhieuDieuChuyenKhoNhanVienTaos)
                .HasForeignKey(d => d.NhanVienTaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PDCK_NhanVienTao");

            entity.HasOne(d => d.NhanVienXacNhan).WithMany(p => p.PhieuDieuChuyenKhoNhanVienXacNhans)
                .HasForeignKey(d => d.NhanVienXacNhanId)
                .HasConstraintName("FK_PDCK_NhanVienXacNhan");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.SanPhamId).HasName("PK__SanPham__05180FF44C432F49");

            entity.ToTable("SanPham");

            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.DonViTinh).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenSanPham).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("DangBan");
            entity.Property(e => e.XuatXu).HasMaxLength(100);

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.DanhMucId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_DanhMuc");

            entity.HasMany(d => d.CongDungs).WithMany(p => p.SanPhams)
                .UsingEntity<Dictionary<string, object>>(
                    "SanPhamCongDung",
                    r => r.HasOne<CongDung>().WithMany()
                        .HasForeignKey("CongDungId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_SPCD_CongDung"),
                    l => l.HasOne<SanPham>().WithMany()
                        .HasForeignKey("SanPhamId")
                        .HasConstraintName("FK_SPCD_SanPham"),
                    j =>
                    {
                        j.HasKey("SanPhamId", "CongDungId").HasName("PK__SanPham___B7758742D231FFA5");
                        j.ToTable("SanPham_CongDung");
                        j.IndexerProperty<int>("SanPhamId").HasColumnName("SanPhamID");
                        j.IndexerProperty<int>("CongDungId").HasColumnName("CongDungID");
                    });

            entity.HasMany(d => d.ThanhPhans).WithMany(p => p.SanPhams)
                .UsingEntity<Dictionary<string, object>>(
                    "SanPhamThanhPhan",
                    r => r.HasOne<ThanhPhan>().WithMany()
                        .HasForeignKey("ThanhPhanId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_SPTP_ThanhPhan"),
                    l => l.HasOne<SanPham>().WithMany()
                        .HasForeignKey("SanPhamId")
                        .HasConstraintName("FK_SPTP_SanPham"),
                    j =>
                    {
                        j.HasKey("SanPhamId", "ThanhPhanId").HasName("PK__SanPham___63AEA661F3E830F7");
                        j.ToTable("SanPham_ThanhPhan");
                        j.IndexerProperty<int>("SanPhamId").HasColumnName("SanPhamID");
                        j.IndexerProperty<int>("ThanhPhanId").HasColumnName("ThanhPhanID");
                    });
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanId).HasName("PK__TaiKhoan__9A124B658AF533F2");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC048276311").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D105342433E498").IsUnique();

            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.MatKhauHash).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.VaiTroId).HasColumnName("VaiTroID");

            entity.HasOne(d => d.VaiTro).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.VaiTroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_VaiTro");
        });

        modelBuilder.Entity<ThanhPhan>(entity =>
        {
            entity.HasKey(e => e.ThanhPhanId).HasName("PK__ThanhPha__6B6A995766B278A5");

            entity.ToTable("ThanhPhan");

            entity.HasIndex(e => e.TenThanhPhan, "UQ__ThanhPha__C866F6649A14621A").IsUnique();

            entity.Property(e => e.ThanhPhanId).HasColumnName("ThanhPhanID");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenThanhPhan).HasMaxLength(100);
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.ThongBaoId).HasName("PK__ThongBao__6E51A53BB57B6B94");

            entity.ToTable("ThongBao");

            entity.Property(e => e.ThongBaoId).HasColumnName("ThongBaoID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);
            entity.Property(e => e.TieuDe).HasMaxLength(150);

            entity.HasOne(d => d.KhachHang).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThongBao_KhachHang");
        });

        modelBuilder.Entity<TinNhan>(entity =>
        {
            entity.HasKey(e => e.TinNhanId).HasName("PK__TinNhan__40CE177CAB3A9104");

            entity.ToTable("TinNhan");

            entity.Property(e => e.TinNhanId).HasColumnName("TinNhanID");
            entity.Property(e => e.CuocHoiThoaiId).HasColumnName("CuocHoiThoaiID");
            entity.Property(e => e.NguoiGui).HasMaxLength(20);
            entity.Property(e => e.ThoiGianGui)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CuocHoiThoai).WithMany(p => p.TinNhans)
                .HasForeignKey(d => d.CuocHoiThoaiId)
                .HasConstraintName("FK_TinNhan_CuocHoiThoai");
        });

        modelBuilder.Entity<TrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.TrangThaiId).HasName("PK__TrangTha__D5BF1E8552F96179");

            entity.ToTable("TrangThaiDonHang");

            entity.HasIndex(e => e.TenTrangThai, "UQ__TrangTha__9489EF661FBFC199").IsUnique();

            entity.Property(e => e.TrangThaiId).HasColumnName("TrangThaiID");
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.VaiTroId).HasName("PK__VaiTro__47758136394E3608");

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaiTro, "UQ__VaiTro__1DA5581435601090").IsUnique();

            entity.Property(e => e.VaiTroId).HasColumnName("VaiTroID");
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
