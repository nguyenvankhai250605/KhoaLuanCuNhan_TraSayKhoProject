using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _service;
        public ThongKeController(IThongKeService service) => _service = service;

        // Chỉ Admin (Quản trị hệ thống) xem được toàn hệ thống.
        // Chủ cửa hàng và Nhân viên luôn bị ép về đúng chi nhánh mình, dù có truyền tham số khác.
        private int? XacDinhChiNhanhDuocPhepXem(int? chiNhanhIdYeuCau)
        {
            if (User.CoQuyenXemToanHeThong())
                return chiNhanhIdYeuCau;

            return User.GetChiNhanhId();
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