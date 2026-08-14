namespace TraSayKho.API.DTOs
{
    public class DonHangDto
    {
        public int DonHangId { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
        public DateTime NgayDatHang { get; set; }
    }

    public class ChiTietSanPhamTrongDonDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class DonHangChiTietDto
    {
        public int DonHangId { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string DiaChiGiaoHang { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
        public DateTime NgayDatHang { get; set; }
        public List<ChiTietSanPhamTrongDonDto> ChiTietSanPhams { get; set; } = new();
    }

    // ==== THÊM CLASS MỚI ====
    public class CapNhatTrangThaiDto
    {
        public string TenTrangThaiMoi { get; set; } = string.Empty;
        // Giá trị hợp lệ: ChoXacNhan, DangXuLy, DangGiao, DaGiao, HoanThanh, DaHuy
    }
}