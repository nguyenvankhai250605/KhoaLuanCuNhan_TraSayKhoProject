using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class ChiNhanh
{
    public int ChiNhanhId { get; set; }

    public string TenChiNhanh { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public bool LaTruSoChinh { get; set; }

    public bool TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<HanMucSanPham> HanMucSanPhams { get; set; } = new List<HanMucSanPham>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<PhieuDieuChuyenKho> PhieuDieuChuyenKhoChiNhanhGuis { get; set; } = new List<PhieuDieuChuyenKho>();

    public virtual ICollection<PhieuDieuChuyenKho> PhieuDieuChuyenKhoChiNhanhNhans { get; set; } = new List<PhieuDieuChuyenKho>();

    public virtual ICollection<ThungHang> ThungHangs { get; set; } = new List<ThungHang>();
}
