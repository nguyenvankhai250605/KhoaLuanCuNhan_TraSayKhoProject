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

    public virtual DbSet<HanMucSanPham> HanMucSanPhams { get; set; }

    public virtual DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }

    public virtual DbSet<DonViSanPham> DonViSanPhams { get; set; }

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

    public virtual DbSet<ThungHang> ThungHangs { get; set; }

    public virtual DbSet<TinNhan> TinNhans { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacGiamGiaXaKho>(entity =>
        {
            entity.HasKey(e => e.BacGiamGiaId).HasName("PK__BacGiamG__E6CEA39BC1CF9A06");

            entity.ToTable("BacGiamGiaXaKho");

            entity.HasIndex(e => e.PhanTramThoiGianConLaiToiDa, "UX_BacGiamGia_Chung_Nguong")
                .IsUnique()
                .HasFilter("([DanhMucID] IS NULL)");

            entity.HasIndex(e => new { e.DanhMucId, e.PhanTramThoiGianConLaiToiDa }, "UX_BacGiamGia_DanhMuc_Nguong")
                .IsUnique()
                .HasFilter("([DanhMucID] IS NOT NULL)");

            entity.Property(e => e.BacGiamGiaId).HasColumnName("BacGiamGiaID");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.MucGiamGiaPhanTram).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PhanTramThoiGianConLaiToiDa).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenBac).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.BacGiamGiaXaKhos)
                .HasForeignKey(d => d.DanhMucId)
                .HasConstraintName("FK_BacGiamGia_DanhMuc");
        });

        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.ChiNhanhId).HasName("PK__ChiNhanh__0AC14C8EE7BBA825");

            entity.ToTable("ChiNhanh");

            entity.HasIndex(e => e.TenChiNhanh, "UQ_ChiNhanh_Ten").IsUnique();

            entity.HasIndex(e => e.LaTruSoChinh, "UX_ChiNhanh_ChiMotTruSoChinh")
                .IsUnique()
                .HasFilter("([LaTruSoChinh]=(1))");

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
            entity.HasKey(e => e.ChiTietDonHangId).HasName("PK__ChiTietD__45B33F83A9B949D4");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.ChiTietDonHangId).HasColumnName("ChiTietDonHangID");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonHangId).HasColumnName("DonHangID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("(CONVERT([decimal](18,2),[SoLuong]*[DonGia]))", true)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.DonHang).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("FK_CTDH_DonHang");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDH_SanPham");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.ChiTietGioHangId).HasName("PK__ChiTietG__EC01138D0662F3F0");

            entity.ToTable("ChiTietGioHang");

            entity.HasIndex(e => new { e.GioHangId, e.SanPhamId }, "UQ_ChiTietGioHang_GioHang_SanPham").IsUnique();

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
            entity.HasKey(e => e.ChiTietId).HasName("PK__ChiTietP__B117E9EA19CD514B");

            entity.ToTable("ChiTietPhieuDieuChuyen");

            entity.HasIndex(e => new { e.PhieuDieuChuyenId, e.ThungId }, "UQ_CTPDC_Phieu_Thung").IsUnique();

            entity.Property(e => e.ChiTietId).HasColumnName("ChiTietID");
            entity.Property(e => e.PhieuDieuChuyenId).HasColumnName("PhieuDieuChuyenID");
            entity.Property(e => e.ThungId).HasColumnName("ThungID");

            entity.HasOne(d => d.PhieuDieuChuyen).WithMany(p => p.ChiTietPhieuDieuChuyens)
                .HasForeignKey(d => d.PhieuDieuChuyenId)
                .HasConstraintName("FK_CTPDC_Phieu");

            entity.HasOne(d => d.Thung).WithMany(p => p.ChiTietPhieuDieuChuyens)
                .HasForeignKey(d => d.ThungId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPDC_ThungHang");
        });

        modelBuilder.Entity<CongDung>(entity =>
        {
            entity.HasKey(e => e.CongDungId).HasName("PK__CongDung__26D88B6AB1E8E3A8");

            entity.ToTable("CongDung");

            entity.HasIndex(e => e.TenCongDung, "UQ_CongDung_Ten").IsUnique();

            entity.Property(e => e.CongDungId).HasColumnName("CongDungID");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenCongDung).HasMaxLength(100);
        });

        modelBuilder.Entity<CuocHoiThoai>(entity =>
        {
            entity.HasKey(e => e.CuocHoiThoaiId).HasName("PK__CuocHoiT__DB92E77E300CC3F6");

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
            entity.HasKey(e => e.DanhGiaId).HasName("PK__DanhGia__52C0CA25B14044DE");

            entity.HasIndex(e => new { e.DonHangId, e.SanPhamId }, "UQ_DanhGia_DonHang_SanPham").IsUnique();

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
            entity.HasKey(e => e.DanhMucId).HasName("PK__DanhMuc__1C53BA7B5F835E83");

            entity.ToTable("DanhMuc");

            entity.HasIndex(e => e.TenDanhMuc, "UQ_DanhMuc_Ten").IsUnique();

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
            entity.HasKey(e => e.DonHangId).HasName("PK__DonHang__D159F4DED101B303");

            entity.ToTable("DonHang");

            entity.HasIndex(e => new { e.ChiNhanhId, e.TrangThaiId }, "IX_DonHang_ChiNhanhID_TrangThaiID");

            entity.HasIndex(e => new { e.KhachHangId, e.NgayDatHang }, "IX_DonHang_KhachHangID_NgayDatHang").IsDescending(false, true);

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
            entity.HasKey(e => e.GioHangId).HasName("PK__GioHang__4242280D08428404");

            entity.ToTable("GioHang");

            entity.HasIndex(e => e.KhachHangId, "UQ_GioHang_KhachHang").IsUnique();

            entity.Property(e => e.GioHangId).HasColumnName("GioHangID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");

            entity.HasOne(d => d.KhachHang).WithOne(p => p.GioHang)
                .HasForeignKey<GioHang>(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_KhachHang");
        });

        modelBuilder.Entity<HanMucSanPham>(entity =>
        {
            entity.HasKey(e => e.HanMucId).HasName("PK__HanMucSa__7048B21ABFE1FA5F");

            entity.ToTable("HanMucSanPham");

            entity.HasIndex(e => new { e.SanPhamId, e.ChiNhanhId }, "UX_HanMuc_SanPham_ChiNhanh")
                .IsUnique()
                .HasFilter("([ChiNhanhID] IS NOT NULL)");

            entity.HasIndex(e => e.SanPhamId, "UX_HanMuc_SanPham_Chung")
                .IsUnique()
                .HasFilter("([ChiNhanhID] IS NULL)");

            entity.Property(e => e.HanMucId).HasColumnName("HanMucID");
            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanhID");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.ChiNhanh).WithMany(p => p.HanMucSanPhams)
                .HasForeignKey(d => d.ChiNhanhId)
                .HasConstraintName("FK_HanMuc_ChiNhanh");

            entity.HasOne(d => d.SanPham).WithOne(p => p.HanMucSanPham)
                .HasForeignKey<HanMucSanPham>(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HanMuc_SanPham");
        });

        modelBuilder.Entity<HinhAnhSanPham>(entity =>
        {
            entity.HasKey(e => e.HinhAnhId).HasName("PK__HinhAnhS__8EF32B7B9CFB7C46");

            entity.ToTable("HinhAnhSanPham");

            entity.HasIndex(e => new { e.SanPhamId, e.ThuTuHienThi }, "UQ_HinhAnh_SanPham_ThuTu").IsUnique();

            entity.Property(e => e.HinhAnhId).HasColumnName("HinhAnhID");
            entity.Property(e => e.DuongDanAnh).HasMaxLength(255);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");

            entity.HasOne(d => d.SanPham).WithMany(p => p.HinhAnhSanPhams)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("FK_HinhAnh_SanPham");
        });

        modelBuilder.Entity<DonViSanPham>(entity =>
        {
            entity.HasKey(e => e.DonViId);

            entity.ToTable("DonViSanPham");

            entity.HasIndex(
                e => new { e.ThungId, e.TrangThai },
                "IX_DonViSanPham_ThungID_TrangThai");

            entity.HasIndex(
                e => e.MaDonVi,
                "UQ_DonViSanPham_MaDonVi")
                .IsUnique();

            entity.Property(e => e.DonViId)
                .HasColumnName("DonViID");

            entity.Property(e => e.ThungId)
                .HasColumnName("ThungID");

            entity.Property(e => e.MaDonVi)
                .HasMaxLength(50);

            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ConKho");

            entity.Property(e => e.ChiTietDonHangId)
                .HasColumnName("ChiTietDonHangID");

            entity.Property(e => e.NgayBan)
                .HasColumnType("datetime");

            entity.HasOne(d => d.ChiTietDonHang)
                .WithMany(p => p.DonViSanPhams)
                .HasForeignKey(d => d.ChiTietDonHangId)
                .HasConstraintName(
                    "FK_DonViSanPham_ChiTietDonHang");

            entity.HasOne(d => d.Thung)
                .WithMany(p => p.DonViSanPhams)
                .HasForeignKey(d => d.ThungId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DonViSanPham_ThungHang");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangId).HasName("PK__KhachHan__880F211BE93B7927");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.TaiKhoanId, "UQ_KhachHang_TaiKhoan").IsUnique();

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
            entity.HasKey(e => e.KhuyenMaiId).HasName("PK__KhuyenMa__820D7477D62E0AF5");

            entity.ToTable("KhuyenMai");

            entity.HasIndex(e => e.MaCode, "UQ_KhuyenMai_MaCode").IsUnique();

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
            entity.HasKey(e => e.LichSuId).HasName("PK__LichSuTr__CD0C1E3B6CB4B612");

            entity.ToTable("LichSuTrangThaiDonHang");

            entity.HasIndex(e => new { e.DonHangId, e.ThoiGianCapNhat }, "IX_LichSuTrangThai_DonHangID");

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
            entity.HasKey(e => e.LoHangId).HasName("PK__LoHang__AAA884846687F0E8");

            entity.ToTable("LoHang");

            entity.HasIndex(e => new { e.SanPhamId, e.HanSuDung }, "IX_LoHang_SanPhamID_HanSuDung");

            entity.HasIndex(e => e.SoLo, "UQ_LoHang_SoLo").IsUnique();

            entity.Property(e => e.LoHangId).HasColumnName("LoHangID");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.SoLo).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ConHang");

            entity.HasOne(d => d.SanPham).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoHang_SanPham");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienId).HasName("PK__NhanVien__E27FD7EAB5CB2291");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.ChiNhanhId, "IX_NhanVien_ChiNhanhID");

            entity.HasIndex(e => e.TaiKhoanId, "UQ_NhanVien_TaiKhoan").IsUnique();

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
            entity.HasKey(e => e.PhieuDieuChuyenId).HasName("PK__PhieuDie__EE393D97C7603F6D");

            entity.ToTable("PhieuDieuChuyenKho");

            entity.HasIndex(e => new { e.ChiNhanhGuiId, e.TrangThai }, "IX_PhieuDieuChuyen_ChiNhanhGui");

            entity.HasIndex(e => new { e.ChiNhanhNhanId, e.TrangThai }, "IX_PhieuDieuChuyen_ChiNhanhNhan");

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
                .HasDefaultValue("ChoDuyet");

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
            entity.HasKey(e => e.SanPhamId).HasName("PK__SanPham__05180FF416411BDE");

            entity.ToTable("SanPham");

            entity.HasIndex(e => new { e.DanhMucId, e.TrangThai }, "IX_SanPham_DanhMucID_TrangThai");

            entity.Property(e => e.SanPhamId).HasColumnName("SanPhamID");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMucID");
            entity.Property(e => e.DonViTinh).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KhoiLuongGam).HasColumnType("decimal(10, 2)");
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
                        j.HasKey("SanPhamId", "CongDungId");
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
                        j.HasKey("SanPhamId", "ThanhPhanId");
                        j.ToTable("SanPham_ThanhPhan");
                        j.IndexerProperty<int>("SanPhamId").HasColumnName("SanPhamID");
                        j.IndexerProperty<int>("ThanhPhanId").HasColumnName("ThanhPhanID");
                    });
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanId).HasName("PK__TaiKhoan__9A124B65FFD34DCD");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.VaiTroId, "IX_TaiKhoan_VaiTroID");

            entity.HasIndex(e => e.Email, "UQ_TaiKhoan_Email").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ_TaiKhoan_TenDangNhap").IsUnique();

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
            entity.HasKey(e => e.ThanhPhanId).HasName("PK__ThanhPha__6B6A99579BF679C4");

            entity.ToTable("ThanhPhan");

            entity.HasIndex(e => e.TenThanhPhan, "UQ_ThanhPhan_Ten").IsUnique();

            entity.Property(e => e.ThanhPhanId).HasColumnName("ThanhPhanID");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenThanhPhan).HasMaxLength(100);
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.ThongBaoId).HasName("PK__ThongBao__6E51A53BAB060686");

            entity.ToTable("ThongBao");

            entity.HasIndex(e => new { e.KhachHangId, e.DaDoc, e.NgayTao }, "IX_ThongBao_KhachHangID_DaDoc").IsDescending(false, false, true);

            entity.Property(e => e.ThongBaoId).HasColumnName("ThongBaoID");
            entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);
            entity.Property(e => e.TieuDe).HasMaxLength(150);

            entity.HasOne(d => d.KhachHang).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("FK_ThongBao_KhachHang");
        });

        modelBuilder.Entity<ThungHang>(entity =>
        {
            entity.HasKey(e => e.ThungId).HasName("PK__ThungHan__D7A2D3AB0DD7BEB4");

            entity.ToTable("ThungHang");

            entity.HasIndex(e => new { e.ChiNhanhId, e.TrangThai }, "IX_ThungHang_ChiNhanhID_TrangThai");

            entity.HasIndex(e => e.MaThung, "UQ_ThungHang_MaThung").IsUnique();

            entity.Property(e => e.ThungId).HasColumnName("ThungID");
            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanhID");
            entity.Property(e => e.LoHangId).HasColumnName("LoHangID");
            entity.Property(e => e.MaThung).HasMaxLength(50);
            entity.Property(e => e.SoLuongDonVi).HasColumnName("SoLuongDonVi");
            entity.Property(e => e.NgayPhanBo).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ConHang");

            entity.HasOne(d => d.ChiNhanh).WithMany(p => p.ThungHangs)
                .HasForeignKey(d => d.ChiNhanhId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThungHang_ChiNhanh");

            entity.HasOne(d => d.LoHang).WithMany(p => p.ThungHangs)
                .HasForeignKey(d => d.LoHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThungHang_LoHang");
        });

        modelBuilder.Entity<TinNhan>(entity =>
        {
            entity.HasKey(e => e.TinNhanId).HasName("PK__TinNhan__40CE177CEB46AE5F");

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
            entity.HasKey(e => e.TrangThaiId).HasName("PK__TrangTha__D5BF1E85C0E4AB50");

            entity.ToTable("TrangThaiDonHang");

            entity.HasIndex(e => e.TenTrangThai, "UQ_TrangThaiDonHang_Ten").IsUnique();

            entity.Property(e => e.TrangThaiId).HasColumnName("TrangThaiID");
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.VaiTroId).HasName("PK__VaiTro__477581364226B198");

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaiTro, "UQ_VaiTro_TenVaiTro").IsUnique();

            entity.Property(e => e.VaiTroId).HasColumnName("VaiTroID");
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
