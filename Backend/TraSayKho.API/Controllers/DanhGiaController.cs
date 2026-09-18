using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TraSayKho.API.DTOs;
using TraSayKho.API.Helpers;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhGiaController : ControllerBase
    {
        private readonly IDanhGiaService _service;
        public DanhGiaController(IDanhGiaService service) => _service = service;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy đánh giá." });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> Create([FromBody] DanhGiaCreateDto dto)
        {
            var khachHangId = User.GetKhachHangId();
            if (!khachHangId.HasValue) return Forbid();

            var (success, errorMessage, result) = await _service.CreateAsync(khachHangId.Value, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.DanhGiaId }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,ChuCuaHang")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy đánh giá." });
            return Ok(new { message = "Đã xóa đánh giá." });
        }
    }
}
