using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class ChiNhanhController : ControllerBase
    {
        private readonly IChiNhanhService _service;
        public ChiNhanhController(IChiNhanhService service) => _service = service;

        // Ai cũng xem được danh sách chi nhánh (cần thiết để chọn lúc tạo Lô hàng, Phiếu điều chuyển...)
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy chi nhánh." });
            return Ok(result);
        }

        // CHỈ Admin mới được tạo/sửa/xóa chi nhánh
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ChiNhanhCreateDto dto)
        {
            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.ChiNhanhId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ChiNhanhUpdateDto dto)
        {
            var (success, errorMessage) = await _service.UpdateAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Cập nhật chi nhánh thành công." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy chi nhánh." });
            return Ok(new { message = "Đã ngừng hoạt động chi nhánh." });
        }
    }
}