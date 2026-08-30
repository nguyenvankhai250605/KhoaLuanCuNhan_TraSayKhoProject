namespace TraSayKho.API.DTOs
{
    public class LoHangDto
    {
        public int LoHangId { get; set; }
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int ChiNhanhId { get; set; }
        public string TenChiNhanh { get; set; } = string.Empty;
        public string SoLo { get; set; } = string.Empty;
        public DateOnly NgayNhap { get; set; }
        public DateOnly HanSuDung { get; set; }
        public int SoLuongNhap { get; set; }
        public int SoLuongConLai { get; set; }
        public decimal? MucGiamGiaHienTai { get; set; }
        public DateOnly? NgayBatDauApDungGiam { get; set; }
        public DateOnly? NgayKetThucApDungGiam { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int SoNgayConLaiDenHan { get; set; }   // tính sẵn, tiện cho Web hiển thị cảnh báo
    }

    public class LoHangCreateDto
    {
        public int SanPhamId { get; set; }
        public int ChiNhanhId { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateOnly? NgayNhap { get; set; }   // để trống = lấy ngày hôm nay
        public DateOnly HanSuDung { get; set; }
        public int SoLuongNhap { get; set; }
    }

    public class XaKhoDto
    {
        public decimal MucGiamGia { get; set; }          // %, ví dụ 15 nghĩa là giảm 15%
        public DateOnly NgayBatDauApDung { get; set; }
        public DateOnly NgayKetThucApDung { get; set; }
    }
}