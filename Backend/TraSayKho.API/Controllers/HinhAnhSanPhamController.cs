using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/SanPham/{sanPhamId}/[controller]")]
    public class HinhAnhSanPhamController : ControllerBase
    {
        private readonly IHinhAnhSanPhamService _service;
        public HinhAnhSanPhamController(IHinhAnhSanPhamService service) => _service = service;

        // GET: api/SanPham/5/HinhAnhSanPham
        [HttpGet]
        public async Task<IActionResult> GetBySanPham(int sanPhamId)
        {
            return Ok(await _service.GetBySanPhamIdAsync(sanPhamId));
        }

        // POST: api/SanPham/5/HinhAnhSanPham
        [HttpPost]
        public async Task<IActionResult> Upload(int sanPhamId, IFormFile file, [FromForm] int thuTuHienThi = 0)
        {
            var (success, errorMessage, result) = await _service.UploadAsync(sanPhamId, file, thuTuHienThi);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        // DELETE: api/SanPham/5/HinhAnhSanPham/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int sanPhamId, int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy hình ảnh." });
            return Ok(new { message = "Đã xóa hình ảnh." });
        }
    }
}