using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class HinhAnhSanPham
{
    public int HinhAnhId { get; set; }

    public int SanPhamId { get; set; }

    public string DuongDanAnh { get; set; } = null!;

    public int ThuTuHienThi { get; set; }

    public virtual SanPham SanPham { get; set; } = null!;
}
