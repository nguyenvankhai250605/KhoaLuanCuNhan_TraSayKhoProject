using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class NhanVien
{
    public int NhanVienId { get; set; }

    public int TaiKhoanId { get; set; }

    public int? ChiNhanhId { get; set; }

    public string HoTen { get; set; } = null!;

    public string? ChucVu { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    public virtual ChiNhanh? ChiNhanh { get; set; }

    public virtual ICollection<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; } = new List<LichSuTrangThaiDonHang>();

    public virtual ICollection<PhieuDieuChuyenKho> PhieuDieuChuyenKhoNhanVienTaos { get; set; } = new List<PhieuDieuChuyenKho>();

    public virtual ICollection<PhieuDieuChuyenKho> PhieuDieuChuyenKhoNhanVienXacNhans { get; set; } = new List<PhieuDieuChuyenKho>();

    public virtual TaiKhoan TaiKhoan { get; set; } = null!;
}
