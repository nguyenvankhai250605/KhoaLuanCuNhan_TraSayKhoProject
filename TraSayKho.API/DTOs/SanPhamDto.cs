namespace TraSayKho.API.DTOs
{
    public class SanPhamDto
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string TenDanhMuc { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }
    public class SanPhamCreateDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int DanhMucId { get; set; }
        public string? MoTaChiTiet { get; set; }
        public string? XuatXu { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string? DonViTinh { get; set; }
        public DateOnly? HanSuDung { get; set; }
    }

    public class SanPhamUpdateDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int DanhMucId { get; set; }
        public string? MoTaChiTiet { get; set; }
        public string? XuatXu { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string? DonViTinh { get; set; }
        public DateOnly? HanSuDung { get; set; }
        public string TrangThai { get; set; } = "DangBan";
    }
}