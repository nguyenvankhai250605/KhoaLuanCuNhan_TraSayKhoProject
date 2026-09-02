using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoService _service;
        public ThongBaoController(IThongBaoService service) => _service = service;

        // GET: api/ThongBao — xem toàn bộ lịch sử thông báo đã gửi (cho Admin)
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        // GET: api/ThongBao/khachhang/5 — xem thông báo của 1 khách hàng cụ thể
        [HttpGet("khachhang/{khachHangId}")]
        public async Task<IActionResult> GetByKhachHang(int khachHangId)
        {
            return Ok(await _service.GetByKhachHangIdAsync(khachHangId));
        }

        // POST: api/ThongBao — gửi thông báo (1 người hoặc tất cả)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThongBaoCreateDto dto)
        {
            var (success, errorMessage, soLuongDaGui) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = $"Đã gửi thông báo tới {soLuongDaGui} khách hàng." });
        }
    }
}