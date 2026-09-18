using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ThungHang
{
    public int ThungId { get; set; }

    public int LoHangId { get; set; }

    public int ChiNhanhId { get; set; }

    public string MaThung { get; set; } = null!;

    public int SoLuongDonVi { get; set; }

    public DateOnly NgayPhanBo { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ChiNhanh ChiNhanh { get; set; } = null!;

    public virtual ICollection<ChiTietPhieuDieuChuyen>
        ChiTietPhieuDieuChuyens { get; set; }
        = new List<ChiTietPhieuDieuChuyen>();

    public virtual ICollection<DonViSanPham>
        DonViSanPhams { get; set; }
        = new List<DonViSanPham>();

    public virtual LoHang LoHang { get; set; } = null!;
}