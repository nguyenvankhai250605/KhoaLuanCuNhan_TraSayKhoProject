namespace TraSayKho.API.DTOs
{
    public class ChatRequestDto
    {
        public int KhachHangId { get; set; }
        public string NoiDung { get; set; } = string.Empty;
    }

    public class ChatResponseDto
    {
        public int CuocHoiThoaiId { get; set; }
        public string NoiDungTraLoi { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
    }

    public class TinNhanDto
    {
        public int TinNhanId { get; set; }
        public string NguoiGui { get; set; } = string.Empty;   // KhachHang hoặc Chatbot
        public string NoiDung { get; set; } = string.Empty;
        public DateTime ThoiGianGui { get; set; }
    }
}