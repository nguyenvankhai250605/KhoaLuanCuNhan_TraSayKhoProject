using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhMucController : ControllerBase
    {
        private readonly IDanhMucService _service;
        public DanhMucController(IDanhMucService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy danh mục." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DanhMucCreateDto dto)
        {
            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.DanhMucId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DanhMucUpdateDto dto)
        {
            var (success, errorMessage) = await _service.UpdateAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Cập nhật danh mục thành công." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy danh mục." });
            return Ok(new { message = "Đã ngừng hoạt động danh mục." });
        }
    }
}