namespace TraSayKho.API.DTOs
{
    public class ChiTietPhieuDieuChuyenDto
    {
        public int ChiTietId { get; set; }
        public int LoHangId { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public string TenSanPham { get; set; } = string.Empty;
        public DateOnly HanSuDung { get; set; }
        public int SoLuong { get; set; }
    }

    public class PhieuDieuChuyenDto
    {
        public int PhieuDieuChuyenId { get; set; }
        public int ChiNhanhGuiId { get; set; }
        public string TenChiNhanhGui { get; set; } = string.Empty;
        public int ChiNhanhNhanId { get; set; }
        public string TenChiNhanhNhan { get; set; } = string.Empty;
        public string TenNhanVienTao { get; set; } = string.Empty;
        public string? TenNhanVienXacNhan { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayXacNhan { get; set; }
        public List<ChiTietPhieuDieuChuyenDto> ChiTiet { get; set; } = new();
    }

    public class DongDeXuatDto
    {
        public int SanPhamId { get; set; }
        public int SoLuong { get; set; }
    }

    public class PhieuDieuChuyenCreateDto
    {
        public int ChiNhanhGuiId { get; set; }    // chi nhánh còn tồn, được chọn để xin hàng
        public int ChiNhanhNhanId { get; set; }   // chi nhánh của người đang tạo yêu cầu (đang thiếu hàng)
        public int NhanVienTaoId { get; set; }
        public string? GhiChu { get; set; }
        public List<DongDeXuatDto> ChiTiet { get; set; } = new();
    }

    public class TuChoiPhieuDto
    {
        public string LyDoTuChoi { get; set; } = string.Empty;
    }

    public class XacNhanThucHienDto
    {
        public int NhanVienId { get; set; }
    }
}