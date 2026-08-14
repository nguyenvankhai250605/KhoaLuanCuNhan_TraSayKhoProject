namespace TraSayKho.API.DTOs
{
    public class KhachHangDto
    {
        public int KhachHangId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public bool TrangThaiTaiKhoan { get; set; }
    }

    public class KhachHangUpdateDto
    {
        public string HoTen { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
    }

    public class KhoaTaiKhoanDto
    {
        public bool TrangThai { get; set; } // true = mở khóa, false = khóa
    }
}