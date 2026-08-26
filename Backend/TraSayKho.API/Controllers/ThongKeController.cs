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

        // GET: api/ThongKe/tongquan
        [HttpGet("tongquan")]
        public async Task<IActionResult> GetTongQuan()
        {
            return Ok(await _service.GetTongQuanAsync());
        }

        // GET: api/ThongKe/doanhthu?tuNgay=2026-08-01&denNgay=2026-08-31
        [HttpGet("doanhthu")]
        public async Task<IActionResult> GetDoanhThuTheoNgay(
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay)
        {
            var ngayBatDau = tuNgay ?? DateTime.Now.AddDays(-30);
            var ngayKetThuc = denNgay ?? DateTime.Now;

            var result = await _service.GetDoanhThuTheoNgayAsync(ngayBatDau, ngayKetThuc);
            return Ok(result);
        }

        // GET: api/ThongKe/sanphambanchay?top=5
        [HttpGet("sanphambanchay")]
        public async Task<IActionResult> GetTopSanPhamBanChay(
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay,
            [FromQuery] int top = 5)
        {
            var ngayBatDau = tuNgay ?? DateTime.Now.AddDays(-30);
            var ngayKetThuc = denNgay ?? DateTime.Now;

            var result = await _service.GetTopSanPhamBanChayAsync(ngayBatDau, ngayKetThuc, top);
            return Ok(result);
        }
    }
}