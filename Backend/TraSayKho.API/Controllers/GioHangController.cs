using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _service;
        public GioHangController(IGioHangService service) => _service = service;

        // GET: api/GioHang/5
        [HttpGet("{khachHangId}")]
        public async Task<IActionResult> Get(int khachHangId)
        {
            var (success, errorMessage, result) = await _service.GetGioHangAsync(khachHangId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        // POST: api/GioHang/5/them
        [HttpPost("{khachHangId}/them")]
        public async Task<IActionResult> ThemVao(int khachHangId, [FromBody] ThemVaoGioHangDto dto)
        {
            var (success, errorMessage) = await _service.ThemVaoGioHangAsync(khachHangId, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã thêm vào giỏ hàng." });
        }

        // PUT: api/GioHang/chitiet/12
        [HttpPut("chitiet/{chiTietId}")]
        public async Task<IActionResult> CapNhatSoLuong(int chiTietId, [FromBody] CapNhatSoLuongGioHangDto dto)
        {
            var (success, errorMessage) = await _service.CapNhatSoLuongAsync(chiTietId, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã cập nhật số lượng." });
        }

        // DELETE: api/GioHang/chitiet/12
        [HttpDelete("chitiet/{chiTietId}")]
        public async Task<IActionResult> XoaChiTiet(int chiTietId)
        {
            var (success, errorMessage) = await _service.XoaChiTietAsync(chiTietId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }

        // DELETE: api/GioHang/5/xoahet
        [HttpDelete("{khachHangId}/xoahet")]
        public async Task<IActionResult> XoaToanBo(int khachHangId)
        {
            var (success, errorMessage) = await _service.XoaToanBoAsync(khachHangId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xóa toàn bộ giỏ hàng." });
        }
    }
}