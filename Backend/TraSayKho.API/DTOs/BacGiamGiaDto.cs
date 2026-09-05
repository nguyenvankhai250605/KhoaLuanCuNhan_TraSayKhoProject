namespace TraSayKho.API.DTOs
{
    public class BacGiamGiaDto
    {
        public int BacGiamGiaId { get; set; }
        public string TenBac { get; set; } = string.Empty;
        public int SoNgayConLaiToiDa { get; set; }
        public decimal MucGiamGiaPhanTram { get; set; }
        public bool TrangThai { get; set; }
    }

    public class BacGiamGiaCreateDto
    {
        public string TenBac { get; set; } = string.Empty;
        public int SoNgayConLaiToiDa { get; set; }
        public decimal MucGiamGiaPhanTram { get; set; }
    }

    public class BacGiamGiaUpdateDto
    {
        public string TenBac { get; set; } = string.Empty;
        public int SoNgayConLaiToiDa { get; set; }
        public decimal MucGiamGiaPhanTram { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}