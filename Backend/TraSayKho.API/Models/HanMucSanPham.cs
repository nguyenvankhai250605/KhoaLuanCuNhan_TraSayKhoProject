using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class HanMucSanPham
{
    public int HanMucId { get; set; }

    public int SanPhamId { get; set; }

    public int? ChiNhanhId { get; set; }

    public int? HanMucBanNgay { get; set; }

    public int? HanMucBanTuan { get; set; }

    public int? TonKhoToiThieu { get; set; }

    public int? TonKhoToiDa { get; set; }

    public int? SoNgayLuuKhoToiDa { get; set; }

    public bool TrangThai { get; set; }

    public virtual ChiNhanh? ChiNhanh { get; set; }

    public virtual SanPham SanPham { get; set; } = null!;
}
