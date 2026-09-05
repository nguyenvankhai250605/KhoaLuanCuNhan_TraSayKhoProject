using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class PhieuDieuChuyenController : ControllerBase
    {
        private readonly IPhieuDieuChuyenService _service;
        public PhieuDieuChuyenController(IPhieuDieuChuyenService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();

            // Nhân viên chỉ thấy phiếu liên quan tới chi nhánh mình (dù là bên gửi hay bên nhận)
            if (!User.LaAdmin())
            {
                var chiNhanhId = User.GetChiNhanhId();
                list = list.Where(p => p.ChiNhanhGuiId == chiNhanhId || p.ChiNhanhNhanId == chiNhanhId).ToList();
            }

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            if (!User.LaAdmin())
            {
                var chiNhanhId = User.GetChiNhanhId();
                if (result.ChiNhanhGuiId != chiNhanhId && result.ChiNhanhNhanId != chiNhanhId)
                    return Forbid();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuDieuChuyenCreateDto dto)
        {
            // Chỉ chi nhánh GỬI mới được tạo phiếu (chủ động gửi hàng đi)
            if (!User.DuocPhepThaoTacChiNhanh(dto.ChiNhanhGuiId))
                return Forbid();

            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.PhieuDieuChuyenId }, result);
        }

        [HttpPut("{id}/xacnhan")]
        public async Task<IActionResult> XacNhan(int id, [FromBody] XacNhanPhieuDto dto)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            // Chỉ chi nhánh NHẬN mới được xác nhận (họ mới biết hàng đã tới nơi thật hay chưa)
            if (!User.DuocPhepThaoTacChiNhanh(phieu.ChiNhanhNhanId))
                return Forbid();

            var (success, errorMessage) = await _service.XacNhanAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xác nhận điều chuyển kho thành công." });
        }

        [HttpPut("{id}/huy")]
        public async Task<IActionResult> Huy(int id)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            // Chỉ chi nhánh GỬI (người tạo phiếu) mới được hủy
            if (!User.DuocPhepThaoTacChiNhanh(phieu.ChiNhanhGuiId))
                return Forbid();

            var (success, errorMessage) = await _service.HuyAsync(id);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã hủy phiếu điều chuyển." });
        }
    }
}