using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class TrangThaiDonHang
{
    public int TrangThaiId { get; set; }

    public string TenTrangThai { get; set; } = null!;

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; } = new List<LichSuTrangThaiDonHang>();
}
