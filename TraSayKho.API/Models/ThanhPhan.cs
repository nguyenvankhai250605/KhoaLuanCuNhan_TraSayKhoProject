using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ThanhPhan
{
    public int ThanhPhanId { get; set; }

    public string TenThanhPhan { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
