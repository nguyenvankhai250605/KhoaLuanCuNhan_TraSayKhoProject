using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class DonHang
{
    public int DonHangId { get; set; }

    public int KhachHangId { get; set; }

    public int? ChiNhanhId { get; set; }

    public int TrangThaiId { get; set; }

    public int? KhuyenMaiId { get; set; }

    public string DiaChiGiaoHang { get; set; } = null!;

    public string SoDienThoaiNhan { get; set; } = null!;

    public string PhuongThucThanhToan { get; set; } = null!;

    public decimal TienHang { get; set; }

    public decimal TienGiamGia { get; set; }

    public decimal TongTien { get; set; }

    public string? GhiChu { get; set; }

    public DateTime NgayDatHang { get; set; }

    public virtual ChiNhanh? ChiNhanh { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual KhuyenMai? KhuyenMai { get; set; }

    public virtual ICollection<LichSuTrangThaiDonHang> LichSuTrangThaiDonHangs { get; set; } = new List<LichSuTrangThaiDonHang>();

    public virtual TrangThaiDonHang TrangThai { get; set; } = null!;
}
