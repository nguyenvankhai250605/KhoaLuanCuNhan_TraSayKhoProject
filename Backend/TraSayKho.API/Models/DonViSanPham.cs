using System;

namespace TraSayKho.API.Models;

public partial class DonViSanPham
{
    public int DonViId { get; set; }

    public int ThungId { get; set; }

    public string MaDonVi { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public int? ChiTietDonHangId { get; set; }

    public DateTime? NgayBan { get; set; }

    public virtual ChiTietDonHang? ChiTietDonHang { get; set; }

    public virtual ThungHang Thung { get; set; } = null!;
}