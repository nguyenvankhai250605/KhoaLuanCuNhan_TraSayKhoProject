using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class DanhGium
{
    public int DanhGiaId { get; set; }

    public int SanPhamId { get; set; }

    public int KhachHangId { get; set; }

    public int DonHangId { get; set; }

    public int SoSao { get; set; }

    public string? NoiDung { get; set; }

    public DateTime NgayDanhGia { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
