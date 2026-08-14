using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class LichSuTrangThaiDonHang
{
    public int LichSuId { get; set; }

    public int DonHangId { get; set; }

    public int TrangThaiId { get; set; }

    public DateTime ThoiGianCapNhat { get; set; }

    public int? NhanVienId { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;

    public virtual NhanVien? NhanVien { get; set; }

    public virtual TrangThaiDonHang TrangThai { get; set; } = null!;
}
