namespace TraSayKho.API.DTOs
{
    public class DanhGiaDto
    {
        public int DanhGiaId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public int SoSao { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayDanhGia { get; set; }
    }
}