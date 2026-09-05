using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _service;
        public ThongKeController(IThongKeService service) => _service = service;

        // Nhân viên bị BẮT BUỘC chỉ xem chi nhánh mình, dù có truyền tham số khác đi nữa
        private int? XacDinhChiNhanhDuocPhepXem(int? chiNhanhIdYeuCau)
        {
            if (User.LaAdmin())
                return chiNhanhIdYeuCau;   // Admin muốn xem gì cũng được, kể cả toàn hệ thống (null)

            return User.GetChiNhanhId();   // Nhân viên luôn bị ép về đúng chi nhánh mình
        }

        [HttpGet("tongquan")]
        public async Task<IActionResult> GetTongQuan([FromQuery] int? chiNhanhId)
        {
            var chiNhanhThucTe = XacDinhChiNhanhDuocPhepXem(chiNhanhId);

            var (success, errorMessage, result) = await _service.GetTongQuanAsync(chiNhanhThucTe);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        [HttpGet("doanhthu")]
        public async Task<IActionResult> GetDoanhThuTheoNgay(
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay,
            [FromQuery] int? chiNhanhId)
        {
            var chiNhanhThucTe = XacDinhChiNhanhDuocPhepXem(chiNhanhId);
            var ngayBatDau = tuNgay ?? DateTime.Now.AddDays(-30);
            var ngayKetThuc = denNgay ?? DateTime.Now;

            var result = await _service.GetDoanhThuTheoNgayAsync(ngayBatDau, ngayKetThuc, chiNhanhThucTe);
            return Ok(result);
        }

        [HttpGet("sanphambanchay")]
        public async Task<IActionResult> GetTopSanPhamBanChay(
            [FromQuery] DateTime? tuNgay,
            [FromQuery] DateTime? denNgay,
            [FromQuery] int top = 5,
            [FromQuery] int? chiNhanhId = null)
        {
            var chiNhanhThucTe = XacDinhChiNhanhDuocPhepXem(chiNhanhId);
            var ngayBatDau = tuNgay ?? DateTime.Now.AddDays(-30);
            var ngayKetThuc = denNgay ?? DateTime.Now;

            var result = await _service.GetTopSanPhamBanChayAsync(ngayBatDau, ngayKetThuc, top, chiNhanhThucTe);
            return Ok(result);
        }
    }
}