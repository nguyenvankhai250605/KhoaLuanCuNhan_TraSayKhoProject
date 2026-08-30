using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class LoHang
{
    public int LoHangId { get; set; }

    public int SanPhamId { get; set; }

    public int ChiNhanhId { get; set; }

    public string SoLo { get; set; } = null!;

    public DateOnly NgayNhap { get; set; }

    public DateOnly HanSuDung { get; set; }

    public int SoLuongNhap { get; set; }

    public int SoLuongConLai { get; set; }

    public decimal? MucGiamGiaHienTai { get; set; }

    public DateOnly? NgayBatDauApDungGiam { get; set; }

    public DateOnly? NgayKetThucApDungGiam { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual ChiNhanh ChiNhanh { get; set; } = null!;

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietPhieuDieuChuyen> ChiTietPhieuDieuChuyens { get; set; } = new List<ChiTietPhieuDieuChuyen>();

    public virtual SanPham SanPham { get; set; } = null!;
}
