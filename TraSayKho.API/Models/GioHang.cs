using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class GioHang
{
    public int GioHangId { get; set; }

    public int KhachHangId { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual KhachHang KhachHang { get; set; } = null!;
}
