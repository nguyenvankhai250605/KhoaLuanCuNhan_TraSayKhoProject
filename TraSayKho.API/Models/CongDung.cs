using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class CongDung
{
    public int CongDungId { get; set; }

    public string TenCongDung { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
