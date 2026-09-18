namespace TraSayKho.API.DTOs
{
    public class LoHangCreateDto
    {
        public int SanPhamId { get; set; }

        public string SoLo { get; set; } = string.Empty;

        public DateOnly NgaySanXuat { get; set; }

        public DateOnly HanSuDung { get; set; }

        public int SoLuongThung { get; set; }

        // Số hộp hoặc gói nguyên vẹn trong mỗi thùng
        public int SoDonViMoiThung { get; set; }
    }

    public class ThungHangDto
    {
        public int ThungId { get; set; }

        public string MaThung { get; set; } = string.Empty;

        public int ChiNhanhId { get; set; }

        public string TenChiNhanh { get; set; } = string.Empty;

        public int SoLuongDonVi { get; set; }

        public int SoLuongConKho { get; set; }

        public string TrangThai { get; set; } = string.Empty;
    }

    public class LoHangDto
    {
        public int LoHangId { get; set; }

        public int SanPhamId { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public string DonViTinh { get; set; } = string.Empty;

        public string SoLo { get; set; } = string.Empty;

        public DateOnly NgaySanXuat { get; set; }

        public DateOnly HanSuDung { get; set; }

        public int TongSoLuongNhap { get; set; }

        public int TongSoLuongConKho { get; set; }

        public string TrangThai { get; set; } = string.Empty;

        public int SoNgayConLaiDenHan { get; set; }

        // Được tính tự động từ bảng BacGiamGiaXaKho,
        // không phải cột được lưu trực tiếp trong LoHang.
        public decimal? MucGiamGiaHienTai { get; set; }

        public bool LaGiamGiaTuDong { get; set; }

        public decimal GiaSauGiam { get; set; }

        public List<ThungHangDto> DanhSachThung { get; set; }
            = new();
    }

    public class TruyXuatNguonGocDto
    {
        public int DonViId { get; set; }

        public string MaDonVi { get; set; } = string.Empty;

        // Cho biết đơn vị vật lý là Hộp hay Gói
        public string DonViTinh { get; set; } = string.Empty;

        public string TrangThaiDonVi { get; set; } = string.Empty;

        public string MaThung { get; set; } = string.Empty;

        public int ChiNhanhPhanBoId { get; set; }

        public string TenChiNhanhPhanBo { get; set; }
            = string.Empty;

        public string SoLo { get; set; } = string.Empty;

        public DateOnly NgaySanXuat { get; set; }

        public DateOnly HanSuDung { get; set; }

        public int SanPhamId { get; set; }

        public string TenSanPham { get; set; } = string.Empty;

        public int? DonHangId { get; set; }

        public DateTime? NgayBan { get; set; }

        public string? TenKhachHangMua { get; set; }

        public int? ChiNhanhBanId { get; set; }

        public string? TenChiNhanhBan { get; set; }
    }
}