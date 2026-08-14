using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class SanPham
{
    public int SanPhamId { get; set; }

    public string TenSanPham { get; set; } = null!;

    public int DanhMucId { get; set; }

    public string? MoTaChiTiet { get; set; }

    public string? XuatXu { get; set; }

    public decimal GiaBan { get; set; }

    public int SoLuongTon { get; set; }

    public string? DonViTinh { get; set; }

    public DateOnly? HanSuDung { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual DanhMuc DanhMuc { get; set; } = null!;

    public virtual ICollection<HinhAnhSanPham> HinhAnhSanPhams { get; set; } = new List<HinhAnhSanPham>();

    public virtual ICollection<CongDung> CongDungs { get; set; } = new List<CongDung>();

    public virtual ICollection<ThanhPhan> ThanhPhans { get; set; } = new List<ThanhPhan>();
}
