namespace TraSayKho.API.DTOs
{
    // Mỗi dòng tương ứng với một thùng được điều chuyển.
    public class ChiTietPhieuDieuChuyenDto
    {
        public int ChiTietId { get; set; }

        public int ThungId { get; set; }

        public string MaThung { get; set; } = string.Empty;

        public int LoHangId { get; set; }

        public string SoLo { get; set; } = string.Empty;

        public int SanPhamId { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public string DonViTinh { get; set; } = string.Empty;

        public DateOnly HanSuDung { get; set; }

        // Tổng số hộp hoặc gói trong thùng.
        public int SoLuongDonVi { get; set; }
    }

    public class PhieuDieuChuyenDto
    {
        public int PhieuDieuChuyenId { get; set; }

        public int ChiNhanhGuiId { get; set; }

        public string TenChiNhanhGui { get; set; }
            = string.Empty;

        public int ChiNhanhNhanId { get; set; }

        public string TenChiNhanhNhan { get; set; }
            = string.Empty;

        public string TenNhanVienTao { get; set; }
            = string.Empty;

        public string? TenNhanVienXacNhan { get; set; }

        public string TrangThai { get; set; } = string.Empty;

        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayXacNhan { get; set; }

        public List<ChiTietPhieuDieuChuyenDto> ChiTiet
            { get; set; } = new();
    }

    // Người dùng yêu cầu điều chuyển theo sản phẩm
    // và số đơn vị cần chuyển.
    // Service sẽ chọn nguyên thùng theo FEFO.
    public class DongDeXuatDto
    {
        public int SanPhamId { get; set; }

        // Số hộp hoặc gói cần điều chuyển.
        public int SoLuong { get; set; }
    }

    public class PhieuDieuChuyenCreateDto
    {
        public int ChiNhanhGuiId { get; set; }

        public int ChiNhanhNhanId { get; set; }

        public int NhanVienTaoId { get; set; }

        public string? GhiChu { get; set; }

        public List<DongDeXuatDto> ChiTiet
            { get; set; } = new();
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