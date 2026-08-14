namespace TraSayKho.API.DTOs
{
    public class DanhMucDto
    {
        public int DanhMucId { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
    }

    // ==== THÊM 2 CLASS MỚI ====
    public class DanhMucCreateDto
    {
        public string TenDanhMuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }

    public class DanhMucUpdateDto
    {
        public string TenDanhMuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}