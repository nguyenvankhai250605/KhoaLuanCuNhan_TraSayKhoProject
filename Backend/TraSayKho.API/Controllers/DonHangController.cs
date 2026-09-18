using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonHangController : ControllerBase
    {
        private readonly IDonHangService _service;
        public DonHangController(IDonHangService service) => _service = service;

        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> Create([FromBody] DonHangCreateDto dto)
        {
            var khachHangId = User.GetKhachHangId();
            if (!khachHangId.HasValue) return Forbid();

            // Không tin KhachHangId do client gửi lên; luôn dùng ID trong JWT.
            dto.KhachHangId = khachHangId.Value;
            var (success, errorMessage, result) = await _service.TaoDonHangAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.DonHangId }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();

            if (!User.LaAdmin())
            {
                var chiNhanhId = User.GetChiNhanhId();
                list = list.Where(dh => dh.ChiNhanhId == chiNhanhId).ToList();
            }

            return Ok(list);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });

            if (!User.LaAdmin())
            {
                if (result.ChiNhanhId == null || !User.DuocPhepThaoTacChiNhanh(result.ChiNhanhId.Value))
                    return Forbid();
            }

            return Ok(result);
        }

        [HttpPut("{id}/trangthai")]
        [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
        public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] CapNhatTrangThaiDto dto)
        {
            var donHang = await _service.GetByIdAsync(id);
            if (donHang == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });

            if (!User.LaAdmin())
            {
                if (donHang.ChiNhanhId == null || !User.DuocPhepThaoTacChiNhanh(donHang.ChiNhanhId.Value))
                    return Forbid();
            }

            var (success, errorMessage) = await _service.CapNhatTrangThaiAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công." });
        }
    }
}
