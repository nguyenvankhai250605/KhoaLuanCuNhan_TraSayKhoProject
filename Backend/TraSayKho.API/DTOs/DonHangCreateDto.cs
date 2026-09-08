namespace TraSayKho.API.DTOs
{
    public class ChiTietDonHangCreateDto
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
    }

    public class DonHangCreateDto
    {
        public int KhachHangId { get; set; }
        public int ChiNhanhId { get; set; }
        public string DiaChiGiaoHang { get; set; } = string.Empty;
        public string SoDienThoaiNhan { get; set; } = string.Empty;
        public string PhuongThucThanhToan { get; set; } = "COD";
        public string? GhiChu { get; set; }
        public string? MaKhuyenMai { get; set; }   // để trống nếu không dùng mã giảm giá
        public List<ChiTietDonHangCreateDto> ChiTiet { get; set; } = new();
    }
}