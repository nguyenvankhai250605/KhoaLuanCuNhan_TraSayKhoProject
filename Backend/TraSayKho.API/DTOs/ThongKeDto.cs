namespace TraSayKho.API.DTOs
{
    public class DoanhThuTheoNgayDto
    {
        public DateOnly Ngay { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoDonHang { get; set; }
    }

    public class SanPhamBanChayDto
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int TongSoLuongBan { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class TongQuanDto
    {
        public decimal TongDoanhThu { get; set; }
        public int TongDonHangHoanThanh { get; set; }
        public int DonHangChoXuLy { get; set; }
        public int TongKhachHang { get; set; }
        public int TongSanPhamDangBan { get; set; }
    }
}