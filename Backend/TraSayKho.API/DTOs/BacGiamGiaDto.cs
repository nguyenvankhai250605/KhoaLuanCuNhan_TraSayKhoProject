namespace TraSayKho.API.DTOs
{
    public class BacGiamGiaDto
    {
        public int BacGiamGiaId { get; set; }
        public int? DanhMucId { get; set; }
        public string? TenDanhMuc { get; set; }   // null = áp dụng chung mọi danh mục
        public string TenBac { get; set; } = string.Empty;
        public decimal PhanTramThoiGianConLaiToiDa { get; set; }
        public decimal MucGiamGiaPhanTram { get; set; }
        public bool TrangThai { get; set; }
    }

    public class BacGiamGiaCreateDto
    {
        public int? DanhMucId { get; set; }   // để trống = áp dụng chung
        public string TenBac { get; set; } = string.Empty;
        public decimal PhanTramThoiGianConLaiToiDa { get; set; }
        public decimal MucGiamGiaPhanTram { get; set; }
    }

    public class BacGiamGiaUpdateDto
    {
        public int? DanhMucId { get; set; }
        public string TenBac { get; set; } = string.Empty;
        public decimal PhanTramThoiGianConLaiToiDa { get; set; }
        public decimal MucGiamGiaPhanTram { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}