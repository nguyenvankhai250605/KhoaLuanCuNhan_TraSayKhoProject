using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class KhuyenMaiController : ControllerBase
    {
        private readonly IKhuyenMaiService _service;
        public KhuyenMaiController(IKhuyenMaiService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy khuyến mãi." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhuyenMaiCreateDto dto)
        {
            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.KhuyenMaiId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KhuyenMaiUpdateDto dto)
        {
            var (success, errorMessage) = await _service.UpdateAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Cập nhật khuyến mãi thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy khuyến mãi." });
            return Ok(new { message = "Đã ngừng khuyến mãi." });
        }
    }
}