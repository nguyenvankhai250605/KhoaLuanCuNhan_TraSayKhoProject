using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class DanhMuc
{
    public int DanhMucId { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public int? DanhMucChaId { get; set; }

    public bool TrangThai { get; set; }

    public virtual DanhMuc? DanhMucCha { get; set; }

    public virtual ICollection<DanhMuc> InverseDanhMucCha { get; set; } = new List<DanhMuc>();

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
