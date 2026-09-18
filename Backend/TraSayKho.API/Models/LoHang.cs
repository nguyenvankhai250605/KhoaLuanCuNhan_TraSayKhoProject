using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class LoHang
{
    public int LoHangId { get; set; }

    public int SanPhamId { get; set; }

    public string SoLo { get; set; } = null!;

    public DateOnly NgaySanXuat { get; set; }

    public DateOnly HanSuDung { get; set; }

    public int TongSoLuongNhap { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual SanPham SanPham { get; set; } = null!;

    public virtual ICollection<ThungHang> ThungHangs { get; set; } = new List<ThungHang>();
}
