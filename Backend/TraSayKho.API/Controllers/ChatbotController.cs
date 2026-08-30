using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _service;
        public ChatbotController(IChatbotService service) => _service = service;

        // POST: api/Chatbot/chat
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto dto)
        {
            var (success, errorMessage, result) = await _service.SendMessageAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        // GET: api/Chatbot/lichsu/5
        [HttpGet("lichsu/{khachHangId}")]
        public async Task<IActionResult> GetLichSu(int khachHangId)
        {
            var (success, errorMessage, result) = await _service.GetLichSuAsync(khachHangId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        // PUT: api/Chatbot/dongphien/5
        [HttpPut("dongphien/{cuocHoiThoaiId}")]
        public async Task<IActionResult> DongPhien(int cuocHoiThoaiId)
        {
            var (success, errorMessage) = await _service.DongPhienAsync(cuocHoiThoaiId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã kết thúc phiên trò chuyện." });
        }
    }
}