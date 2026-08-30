using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ChiTietPhieuDieuChuyen
{
    public int ChiTietId { get; set; }

    public int PhieuDieuChuyenId { get; set; }

    public int LoHangId { get; set; }

    public int SoLuong { get; set; }

    public virtual LoHang LoHang { get; set; } = null!;

    public virtual PhieuDieuChuyenKho PhieuDieuChuyen { get; set; } = null!;
}
