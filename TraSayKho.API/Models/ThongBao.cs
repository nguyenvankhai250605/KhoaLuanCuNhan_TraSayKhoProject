using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ThongBao
{
    public int ThongBaoId { get; set; }

    public int KhachHangId { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? NoiDung { get; set; }

    public bool DaDoc { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual KhachHang KhachHang { get; set; } = null!;
}
