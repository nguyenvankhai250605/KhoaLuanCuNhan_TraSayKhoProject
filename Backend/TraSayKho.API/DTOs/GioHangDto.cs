namespace TraSayKho.API.DTOs
{
    public class ChiTietGioHangDto
    {
        public int ChiTietGioHangId { get; set; }
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
        public bool ConHang { get; set; }   // cảnh báo nếu sản phẩm đã hết hàng/ngừng bán
    }

    public class GioHangDto
    {
        public int GioHangId { get; set; }
        public int KhachHangId { get; set; }
        public List<ChiTietGioHangDto> ChiTiet { get; set; } = new();
        public decimal TongTienTamTinh { get; set; }
    }

    public class ThemVaoGioHangDto
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
    }

    public class CapNhatSoLuongGioHangDto
    {
        public int SoLuong { get; set; }
    }
}