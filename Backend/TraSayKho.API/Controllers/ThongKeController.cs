using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _service;
        public ThongKeController(IThongKeService service) => _service = service;

        // GET: api/ThongKe/tongquan?chiNhanhId=1  (bỏ trống chiNhanhId = xem toàn hệ thống)
        [HttpGet("tongquan")]
        public async Task<IActionResult> GetTongQuan([FromQuery] int? chiNhanhId)
        {
            var (success, errorMessage, result) = await _service.GetTongQuanAsync(chiNhanhId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        // GET: api/ThongKe/doanhthu?tuNgay=...&denNgay=...&chiNhanhId=1
        [HttpGet("doanhthu")]
        public async Task<IActionResult> GetDoanhThuTheoNgay(
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay,
            [FromQuery] int? chiNhanhId)
        {
            var ngayBatDau = tuNgay ?? DateTime.Now.AddDays(-30);
            var ngayKetThuc = denNgay ?? DateTime.Now;

            var result = await _service.GetDoanhThuTheoNgayAsync(ngayBatDau, ngayKetThuc, chiNhanhId);
            return Ok(result);
        }

        // GET: api/ThongKe/sanphambanchay?top=5&chiNhanhId=1
        [HttpGet("sanphambanchay")]
        public async Task<IActionResult> GetTopSanPhamBanChay(
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay,
            [FromQuery] int top = 5,
            [FromQuery] int? chiNhanhId = null)
        {
            var ngayBatDau = tuNgay ?? DateTime.Now.AddDays(-30);
            var ngayKetThuc = denNgay ?? DateTime.Now;

            var result = await _service.GetTopSanPhamBanChayAsync(ngayBatDau, ngayKetThuc, top, chiNhanhId);
            return Ok(result);
        }
    }
}