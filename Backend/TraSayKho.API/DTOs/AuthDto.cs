namespace TraSayKho.API.DTOs
{
    // Khách hàng tự đăng ký qua app
    public class DangKyDto
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
    }

    // Admin tổng tạo tài khoản Nhân viên/Admin mới
    public class TaoTaiKhoanNhanVienDto
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? ChucVu { get; set; }
        public int? ChiNhanhId { get; set; }   // để trống = Admin tổng
        public string TenVaiTro { get; set; } = "NhanVien";   // "NhanVien" hoặc "Admin"
    }

    public class DangNhapDto
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }

    public class DangNhapResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public int? ChiNhanhId { get; set; }
        public string? TenChiNhanh { get; set; }
        public DateTime ThoiGianHetHan { get; set; }
    }
}