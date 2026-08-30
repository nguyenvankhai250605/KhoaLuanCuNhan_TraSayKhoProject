namespace TraSayKho.API.DTOs
{
    public class ChiNhanhDto
    {
        public int ChiNhanhId { get; set; }
        public string TenChiNhanh { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public bool LaTruSoChinh { get; set; }
        public bool TrangThai { get; set; }
    }

    public class ChiNhanhCreateDto
    {
        public string TenChiNhanh { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
    }

    public class ChiNhanhUpdateDto
    {
        public string TenChiNhanh { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}