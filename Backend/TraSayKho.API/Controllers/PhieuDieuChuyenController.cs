using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhieuDieuChuyenController : ControllerBase
    {
        private readonly IPhieuDieuChuyenService _service;
        public PhieuDieuChuyenController(IPhieuDieuChuyenService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuDieuChuyenCreateDto dto)
        {
            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.PhieuDieuChuyenId }, result);
        }

        [HttpPut("{id}/xacnhan")]
        public async Task<IActionResult> XacNhan(int id, [FromBody] XacNhanPhieuDto dto)
        {
            var (success, errorMessage) = await _service.XacNhanAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xác nhận điều chuyển kho thành công." });
        }

        [HttpPut("{id}/huy")]
        public async Task<IActionResult> Huy(int id)
        {
            var (success, errorMessage) = await _service.HuyAsync(id);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã hủy phiếu điều chuyển." });
        }
    }
}