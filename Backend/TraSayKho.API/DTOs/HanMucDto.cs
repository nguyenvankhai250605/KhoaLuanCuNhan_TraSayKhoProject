namespace TraSayKho.API.DTOs
{
    public class HanMucDto
    {
        public int HanMucId { get; set; }
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int? ChiNhanhId { get; set; }
        public string? TenChiNhanh { get; set; }
        public int? HanMucBanNgay { get; set; }
        public int? HanMucBanTuan { get; set; }
        public int? TonKhoToiThieu { get; set; }
        public int? TonKhoToiDa { get; set; }
        public int? SoNgayLuuKhoToiDa { get; set; }
        public bool TrangThai { get; set; }
    }

    public class HanMucCreateDto
    {
        public int SanPhamId { get; set; }
        public int? ChiNhanhId { get; set; }
        public int? HanMucBanNgay { get; set; }
        public int? HanMucBanTuan { get; set; }
        public int? TonKhoToiThieu { get; set; }
        public int? TonKhoToiDa { get; set; }
        public int? SoNgayLuuKhoToiDa { get; set; }
    }

    public class HanMucUpdateDto
    {
        public int? HanMucBanNgay { get; set; }
        public int? HanMucBanTuan { get; set; }
        public int? TonKhoToiThieu { get; set; }
        public int? TonKhoToiDa { get; set; }
        public int? SoNgayLuuKhoToiDa { get; set; }
        public bool TrangThai { get; set; } = true;
    }

    public class CanhBaoTonKhoDto
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int TonKhoHienTai { get; set; }
        public int? TonKhoToiThieu { get; set; }
        public int? TonKhoToiDa { get; set; }
        public string LoaiCanhBao { get; set; } = string.Empty;   // "DuoiMuc" hoặc "VuotMuc"
    }
}