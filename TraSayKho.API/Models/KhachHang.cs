using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class KhachHang
{
    public int KhachHangId { get; set; }

    public int TaiKhoanId { get; set; }

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? DiaChi { get; set; }

    public string? AvatarUrl { get; set; }

    public virtual ICollection<CuocHoiThoai> CuocHoiThoais { get; set; } = new List<CuocHoiThoai>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual GioHang? GioHang { get; set; }

    public virtual TaiKhoan TaiKhoan { get; set; } = null!;

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
