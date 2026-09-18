using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ChiTietPhieuDieuChuyen
{
    public int ChiTietId { get; set; }

    public int PhieuDieuChuyenId { get; set; }

    public int ThungId { get; set; }

    public virtual PhieuDieuChuyenKho PhieuDieuChuyen { get; set; } = null!;

    public virtual ThungHang Thung { get; set; } = null!;
}
