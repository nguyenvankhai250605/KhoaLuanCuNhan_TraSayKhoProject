using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class KhuyenMai
{
    public int KhuyenMaiId { get; set; }

    public string MaCode { get; set; } = null!;

    public string? MoTa { get; set; }

    public string LoaiGiam { get; set; } = null!;

    public decimal GiaTriGiam { get; set; }

    public decimal GiaTriDonHangToiThieu { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public int SoLuotSuDungToiDa { get; set; }

    public int SoLuotDaSuDung { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
