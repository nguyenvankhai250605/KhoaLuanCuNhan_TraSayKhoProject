using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class PhieuDieuChuyenKho
{
    public int PhieuDieuChuyenId { get; set; }

    public int ChiNhanhGuiId { get; set; }

    public int ChiNhanhNhanId { get; set; }

    public int NhanVienTaoId { get; set; }

    public int? NhanVienXacNhanId { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayXacNhan { get; set; }

    public virtual ChiNhanh ChiNhanhGui { get; set; } = null!;

    public virtual ChiNhanh ChiNhanhNhan { get; set; } = null!;

    public virtual ICollection<ChiTietPhieuDieuChuyen> ChiTietPhieuDieuChuyens { get; set; } = new List<ChiTietPhieuDieuChuyen>();

    public virtual NhanVien NhanVienTao { get; set; } = null!;

    public virtual NhanVien? NhanVienXacNhan { get; set; }
}
