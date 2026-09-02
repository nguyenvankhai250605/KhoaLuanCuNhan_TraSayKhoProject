using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service) => _service = service;

        [HttpPost("dangky")]
        public async Task<IActionResult> DangKy([FromBody] DangKyDto dto)
        {
            var (success, errorMessage) = await _service.DangKyKhachHangAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đăng ký thành công. Vui lòng đăng nhập." });
        }

        [HttpPost("taonhanvien")]
        public async Task<IActionResult> TaoNhanVien([FromBody] TaoTaiKhoanNhanVienDto dto)
        {
            var (success, errorMessage) = await _service.TaoNhanVienAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Tạo tài khoản nhân viên thành công." });
        }

        [HttpPost("dangnhap")]
        public async Task<IActionResult> DangNhap([FromBody] DangNhapDto dto)
        {
            var (success, errorMessage, result) = await _service.DangNhapAsync(dto);
            if (!success) return Unauthorized(new { message = errorMessage });
            return Ok(result);
        }
    }
}