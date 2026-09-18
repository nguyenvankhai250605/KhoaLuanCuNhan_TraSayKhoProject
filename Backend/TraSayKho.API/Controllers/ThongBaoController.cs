using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TraSayKho.API.Helpers;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoService _service;
        public ThongBaoController(IThongBaoService service) => _service = service;

        // GET: api/ThongBao — xem toàn bộ lịch sử thông báo đã gửi (cho Admin)
        [HttpGet]
        [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("cuatoi")]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> GetCuaToi()
        {
            var khachHangId = User.GetKhachHangId();
            if (!khachHangId.HasValue) return Forbid();
            return Ok(await _service.GetByKhachHangIdAsync(khachHangId.Value));
        }

        // GET: api/ThongBao/khachhang/5 — xem thông báo của 1 khách hàng cụ thể
        [HttpGet("khachhang/{khachHangId}")]
        [Authorize(Roles = "Admin,NhanVien,ChuCuaHang,KhachHang")]
        public async Task<IActionResult> GetByKhachHang(int khachHangId)
        {
            if (User.IsInRole("KhachHang") && User.GetKhachHangId() != khachHangId)
                return Forbid();
            return Ok(await _service.GetByKhachHangIdAsync(khachHangId));
        }

        // POST: api/ThongBao — gửi thông báo (1 người hoặc tất cả)
        [HttpPost]
        [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
        public async Task<IActionResult> Create([FromBody] ThongBaoCreateDto dto)
        {
            var (success, errorMessage, soLuongDaGui) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = $"Đã gửi thông báo tới {soLuongDaGui} khách hàng." });
        }
    }
}
