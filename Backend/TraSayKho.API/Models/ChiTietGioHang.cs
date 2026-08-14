using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ChiTietGioHang
{
    public int ChiTietGioHangId { get; set; }

    public int GioHangId { get; set; }

    public int SanPhamId { get; set; }

    public int SoLuong { get; set; }

    public DateTime NgayThem { get; set; }

    public virtual GioHang GioHang { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
