using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class LoHangController : ControllerBase
    {
        private readonly ILoHangService _service;
        public LoHangController(ILoHangService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy lô hàng." });
            return Ok(result);
        }

        // GET: api/LoHang/sanpham/5
        [HttpGet("sanpham/{sanPhamId}")]
        public async Task<IActionResult> GetBySanPham(int sanPhamId) => Ok(await _service.GetBySanPhamAsync(sanPhamId));

        // GET: api/LoHang/saphethan?soNgay=30
        [HttpGet("saphethan")]
        public async Task<IActionResult> GetSapHetHan([FromQuery] int soNgay = 30) => Ok(await _service.GetSapHetHanAsync(soNgay));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoHangCreateDto dto)
        {
            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.LoHangId }, result);
        }

        // PUT: api/LoHang/5/xakho
        [HttpPut("{id}/xakho")]
        public async Task<IActionResult> BatXaKho(int id, [FromBody] XaKhoDto dto)
        {
            var (success, errorMessage) = await _service.BatXaKhoAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã bật xả kho cho lô hàng." });
        }

        // PUT: api/LoHang/5/huyxakho
        [HttpPut("{id}/huyxakho")]
        public async Task<IActionResult> HuyXaKho(int id)
        {
            var (success, errorMessage) = await _service.HuyXaKhoAsync(id);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã hủy xả kho, quay về giá gốc." });
        }
    }
}