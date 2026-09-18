using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "KhachHang")]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _service;
        public GioHangController(IGioHangService service) => _service = service;

        private int? KhachHangDangNhap() => User.GetKhachHangId();

        [HttpGet]
        public async Task<IActionResult> GetCuaToi()
        {
            var khachHangId = KhachHangDangNhap();
            if (!khachHangId.HasValue) return Forbid();
            var (success, errorMessage, result) = await _service.GetGioHangAsync(khachHangId.Value);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        [HttpGet("{khachHangId:int}")]
        public async Task<IActionResult> Get(int khachHangId)
        {
            if (KhachHangDangNhap() != khachHangId) return Forbid();
            var (success, errorMessage, result) = await _service.GetGioHangAsync(khachHangId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(result);
        }

        [HttpPost("them")]
        public async Task<IActionResult> ThemVaoCuaToi([FromBody] ThemVaoGioHangDto dto)
        {
            var khachHangId = KhachHangDangNhap();
            if (!khachHangId.HasValue) return Forbid();
            var (success, errorMessage) = await _service.ThemVaoGioHangAsync(khachHangId.Value, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã thêm vào giỏ hàng." });
        }

        [HttpPost("{khachHangId:int}/them")]
        public async Task<IActionResult> ThemVao(int khachHangId, [FromBody] ThemVaoGioHangDto dto)
        {
            if (KhachHangDangNhap() != khachHangId) return Forbid();
            return await ThemVaoCuaToi(dto);
        }

        [HttpPut("chitiet/{chiTietId:int}")]
        public async Task<IActionResult> CapNhatSoLuong(int chiTietId, [FromBody] CapNhatSoLuongGioHangDto dto)
        {
            if (!await ChiTietThuocGioHangCuaToi(chiTietId)) return Forbid();
            var (success, errorMessage) = await _service.CapNhatSoLuongAsync(chiTietId, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã cập nhật số lượng." });
        }

        [HttpDelete("chitiet/{chiTietId:int}")]
        public async Task<IActionResult> XoaChiTiet(int chiTietId)
        {
            if (!await ChiTietThuocGioHangCuaToi(chiTietId)) return Forbid();
            var (success, errorMessage) = await _service.XoaChiTietAsync(chiTietId);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }

        [HttpDelete("xoahet")]
        public async Task<IActionResult> XoaToanBoCuaToi()
        {
            var khachHangId = KhachHangDangNhap();
            if (!khachHangId.HasValue) return Forbid();
            var (success, errorMessage) = await _service.XoaToanBoAsync(khachHangId.Value);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xóa toàn bộ giỏ hàng." });
        }

        [HttpDelete("{khachHangId:int}/xoahet")]
        public async Task<IActionResult> XoaToanBo(int khachHangId)
        {
            if (KhachHangDangNhap() != khachHangId) return Forbid();
            return await XoaToanBoCuaToi();
        }

        private async Task<bool> ChiTietThuocGioHangCuaToi(int chiTietId)
        {
            var khachHangId = KhachHangDangNhap();
            if (!khachHangId.HasValue) return false;
            var (success, _, gioHang) = await _service.GetGioHangAsync(khachHangId.Value);
            return success && gioHang != null &&
                   gioHang.ChiTiet.Any(ct => ct.ChiTietGioHangId == chiTietId);
        }
    }
}
