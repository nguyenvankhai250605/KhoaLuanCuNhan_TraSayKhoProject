namespace TraSayKho.API.DTOs
{
    public class KhuyenMaiDto
    {
        public int KhuyenMaiId { get; set; }
        public string MaCode { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }
    }

    // ==== THÊM 2 CLASS MỚI ====
    public class KhuyenMaiCreateDto
    {
        public string MaCode { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string LoaiGiam { get; set; } = "PhanTram"; // "PhanTram" hoặc "SoTien"
        public decimal GiaTriGiam { get; set; }
        public decimal GiaTriDonHangToiThieu { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SoLuotSuDungToiDa { get; set; } = 1;
    }

    public class KhuyenMaiUpdateDto
    {
        public string MoTa { get; set; } = string.Empty;
        public string LoaiGiam { get; set; } = "PhanTram";
        public decimal GiaTriGiam { get; set; }
        public decimal GiaTriDonHangToiThieu { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SoLuotSuDungToiDa { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}