using System.Text;
using System.Text.Json;
using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class ChatbotService : IChatbotService
    {
        private readonly IChatbotRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private const int SoLuongTinNhanNgheGanNhat = 10;

        public ChatbotService(
            IChatbotRepository repository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<(bool Success, string? ErrorMessage, ChatResponseDto? Result)> SendMessageAsync(ChatRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NoiDung))
                return (false, "Nội dung tin nhắn không được để trống.", null);

            if (!await _repository.KhachHangExistsAsync(dto.KhachHangId))
                return (false, "Khách hàng không tồn tại.", null);

            var apiKey = _configuration["GeminiApi:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return (false, "Chatbot hiện chưa được cấu hình (thiếu API key). Vui lòng liên hệ quản trị viên.", null);

            // 1. Lấy hoặc tạo cuộc hội thoại
            var cuocHoiThoai = await _repository.GetOrCreateCuocHoiThoaiAsync(dto.KhachHangId);

            // 2. Lưu tin nhắn của khách hàng trước
            await _repository.AddTinNhanAsync(cuocHoiThoai.CuocHoiThoaiId, "KhachHang", dto.NoiDung);

            // 3. Chuẩn bị dữ liệu gửi cho AI: kho tri thức sản phẩm + lịch sử hội thoại gần nhất
            var danhSachSanPham = await _repository.GetSanPhamDangBanKemChiTietAsync();
            var lichSuGanNhat = await _repository.GetLichSuGanNhatAsync(cuocHoiThoai.CuocHoiThoaiId, SoLuongTinNhanNgheGanNhat);

            var systemPrompt = BuildSystemPrompt(danhSachSanPham);

            // 4. Gọi Gemini API
            var (thanhCong, loi, cauTraLoi) = await GoiGeminiApiAsync(apiKey, systemPrompt, lichSuGanNhat);

            if (!thanhCong)
                return (false, loi, null);

            // 5. Lưu câu trả lời của AI
            var tinNhanTraLoi = await _repository.AddTinNhanAsync(cuocHoiThoai.CuocHoiThoaiId, "Chatbot", cauTraLoi!);

            return (true, null, new ChatResponseDto
            {
                CuocHoiThoaiId = cuocHoiThoai.CuocHoiThoaiId,
                NoiDungTraLoi = cauTraLoi!,
                ThoiGian = tinNhanTraLoi.ThoiGianGui
            });
        }

        public async Task<(bool Success, string? ErrorMessage, List<TinNhanDto>? Result)> GetLichSuAsync(int khachHangId)
        {
            if (!await _repository.KhachHangExistsAsync(khachHangId))
                return (false, "Khách hàng không tồn tại.", null);

            var cuocHoiThoai = await _repository.GetOrCreateCuocHoiThoaiAsync(khachHangId);
            var lichSu = await _repository.GetToanBoLichSuAsync(cuocHoiThoai.CuocHoiThoaiId);

            var result = lichSu.Select(tn => new TinNhanDto
            {
                TinNhanId = tn.TinNhanId,
                NguoiGui = tn.NguoiGui,
                NoiDung = tn.NoiDung,
                ThoiGianGui = tn.ThoiGianGui
            }).ToList();

            return (true, null, result);
        }

        // ==== Xây dựng "kho tri thức" sản phẩm cho AI ====
        private static string BuildSystemPrompt(List<SanPham> danhSachSanPham)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Bạn là trợ lý tư vấn của cửa hàng trà sấy khô TraSayKho. Nhiệm vụ của bạn là tư vấn loại trà phù hợp với nhu cầu, sở thích và mục đích sử dụng của khách hàng.");
            sb.AppendLine("QUY TẮC BẮT BUỘC:");
            sb.AppendLine("1. CHỈ được giới thiệu các sản phẩm có trong danh sách dưới đây. TUYỆT ĐỐI không tự bịa ra sản phẩm không có trong danh sách.");
            sb.AppendLine("2. Trả lời ngắn gọn, thân thiện, bằng tiếng Việt.");
            sb.AppendLine("3. Nếu khách hỏi điều gì đó không liên quan đến trà hoặc cửa hàng, lịch sự từ chối và hướng khách quay lại chủ đề tư vấn trà.");
            sb.AppendLine();
            sb.AppendLine("DANH SÁCH SẢN PHẨM ĐANG BÁN:");

            foreach (var sp in danhSachSanPham)
            {
                var congDung = sp.CongDungs.Any()
                    ? string.Join(", ", sp.CongDungs.Select(cd => cd.TenCongDung))
                    : "Chưa cập nhật";
                var thanhPhan = sp.ThanhPhans.Any()
                    ? string.Join(", ", sp.ThanhPhans.Select(tp => tp.TenThanhPhan))
                    : "Chưa cập nhật";

                sb.AppendLine($"- {sp.TenSanPham} ({sp.DanhMuc.TenDanhMuc}, {sp.GiaBan:N0}đ): Công dụng: {congDung}. Thành phần: {thanhPhan}.");
            }

            return sb.ToString();
        }

        // ==== Gọi Gemini API ====
        private async Task<(bool Success, string? ErrorMessage, string? CauTraLoi)> GoiGeminiApiAsync(
            string apiKey, string systemPrompt, List<TinNhan> lichSu)
        {
            var model = _configuration["GeminiApi:Model"] ?? "gemini-2.0-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

            // Chuyển lịch sử hội thoại sang định dạng Gemini yêu cầu
            var contents = lichSu.Select(tn => new
            {
                role = tn.NguoiGui == "KhachHang" ? "user" : "model",
                parts = new[] { new { text = tn.NoiDung } }
            }).ToList();

            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("x-goog-api-key", apiKey);

                var response = await httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[DEBUG] URL gọi: {url}");
                    Console.WriteLine($"[DEBUG] Gemini trả về: {responseBody}");

                    if ((int)response.StatusCode == 429)
                        return (false, "Chatbot đang tạm thời quá tải (vượt hạn mức miễn phí). Vui lòng thử lại sau ít phút.", null);

                    return (false, $"Chatbot gặp lỗi khi kết nối tới AI (mã lỗi {(int)response.StatusCode}).", null);
                }

                using var doc = JsonDocument.Parse(responseBody);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return (true, null, text ?? "Xin lỗi, tôi chưa thể trả lời câu hỏi này.");
            }
            catch (HttpRequestException)
            {
                return (false, "Không thể kết nối tới dịch vụ AI. Vui lòng kiểm tra kết nối mạng.", null);
            }
            catch (Exception)
            {
                return (false, "Đã xảy ra lỗi không xác định khi xử lý yêu cầu chatbot.", null);
            }
        }
    }
}