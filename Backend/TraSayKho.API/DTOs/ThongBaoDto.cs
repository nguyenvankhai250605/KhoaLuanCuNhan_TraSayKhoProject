namespace TraSayKho.API.DTOs
{
    public class ThongBaoDto
    {
        public int ThongBaoId { get; set; }
        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string TieuDe { get; set; } = string.Empty;
        public string? NoiDung { get; set; }
        public bool DaDoc { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class ThongBaoCreateDto
    {
        public int? KhachHangId { get; set; }  // null = gửi cho TẤT CẢ khách hàng
        public string TieuDe { get; set; } = string.Empty;
        public string? NoiDung { get; set; }
    }
}