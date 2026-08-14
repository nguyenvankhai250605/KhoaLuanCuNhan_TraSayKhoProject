using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _service;
        public KhachHangController(IKhachHangService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy khách hàng." });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KhachHangUpdateDto dto)
        {
            var (success, errorMessage) = await _service.UpdateAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Cập nhật thông tin khách hàng thành công." });
        }

        [HttpPut("{id}/trangthai")]
        public async Task<IActionResult> SetTrangThai(int id, [FromBody] KhoaTaiKhoanDto dto)
        {
            var success = await _service.SetTrangThaiTaiKhoanAsync(id, dto.TrangThai);
            if (!success) return NotFound(new { message = "Không tìm thấy khách hàng." });

            var thongDiep = dto.TrangThai ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.";
            return Ok(new { message = thongDiep });
        }
    }
}