using System;
using System.Collections.Generic;

namespace TraSayKho.API.Models;

public partial class TaiKhoan
{
    public int TaiKhoanId { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public int VaiTroId { get; set; }

    public bool TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual KhachHang? KhachHang { get; set; }

    public virtual NhanVien? NhanVien { get; set; }

    public virtual VaiTro VaiTro { get; set; } = null!;
}
