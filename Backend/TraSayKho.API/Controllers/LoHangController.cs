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
    public class LoHangController : ControllerBase
    {
        private readonly ILoHangService _service;
        public LoHangController(ILoHangService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();

            // Nhân viên chỉ thấy lô hàng thuộc chi nhánh mình, Admin thấy tất cả
            if (!User.LaAdmin())
            {
                var chiNhanhId = User.GetChiNhanhId();
                list = list.Where(lh => lh.ChiNhanhId == chiNhanhId).ToList();
            }

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy lô hàng." });

            if (!User.DuocPhepThaoTacChiNhanh(result.ChiNhanhId))
                return Forbid();

            return Ok(result);
        }

        [HttpGet("sanpham/{sanPhamId}")]
        public async Task<IActionResult> GetBySanPham(int sanPhamId)
        {
            var list = await _service.GetBySanPhamAsync(sanPhamId);

            if (!User.LaAdmin())
            {
                var chiNhanhId = User.GetChiNhanhId();
                list = list.Where(lh => lh.ChiNhanhId == chiNhanhId).ToList();
            }

            return Ok(list);
        }

        [HttpGet("saphethan")]
        public async Task<IActionResult> GetSapHetHan([FromQuery] int soNgay = 30)
        {
            var list = await _service.GetSapHetHanAsync(soNgay);

            if (!User.LaAdmin())
            {
                var chiNhanhId = User.GetChiNhanhId();
                list = list.Where(lh => lh.ChiNhanhId == chiNhanhId).ToList();
            }

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoHangCreateDto dto)
        {
            // Nhân viên chỉ được nhập lô cho đúng chi nhánh mình
            if (!User.DuocPhepThaoTacChiNhanh(dto.ChiNhanhId))
                return Forbid();

            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.LoHangId }, result);
        }

        [HttpPut("{id}/xakho")]
        public async Task<IActionResult> BatXaKho(int id, [FromBody] XaKhoDto dto)
        {
            var loHang = await _service.GetByIdAsync(id);
            if (loHang == null) return NotFound(new { message = "Không tìm thấy lô hàng." });

            if (!User.DuocPhepThaoTacChiNhanh(loHang.ChiNhanhId))
                return Forbid();

            var (success, errorMessage) = await _service.BatXaKhoAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã bật xả kho cho lô hàng." });
        }

        [HttpPut("{id}/huyxakho")]
        public async Task<IActionResult> HuyXaKho(int id)
        {
            var loHang = await _service.GetByIdAsync(id);
            if (loHang == null) return NotFound(new { message = "Không tìm thấy lô hàng." });

            if (!User.DuocPhepThaoTacChiNhanh(loHang.ChiNhanhId))
                return Forbid();

            var (success, errorMessage) = await _service.HuyXaKhoAsync(id);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã hủy xả kho, quay về giá gốc." });
        }
    }
}