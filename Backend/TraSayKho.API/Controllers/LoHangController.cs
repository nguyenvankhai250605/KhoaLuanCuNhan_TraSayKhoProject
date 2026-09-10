using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,NhanVien,ChuCuaHang")]
    public class LoHangController : ControllerBase
    {
        private readonly ILoHangService _service;
        public LoHangController(ILoHangService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();

            if (!User.CoQuyenXemToanHeThong())
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

            if (!User.CoQuyenXemToanHeThong())
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

            if (!User.CoQuyenXemToanHeThong())
            {
                var chiNhanhId = User.GetChiNhanhId();
                list = list.Where(lh => lh.ChiNhanhId == chiNhanhId).ToList();
            }

            return Ok(list);
        }

        // Nhập lô mới — CHỈ Quản lý cửa hàng / Admin (không phải việc của Nhân viên)
        [HttpPost]
        [Authorize(Roles = "Admin,ChuCuaHang")]
        public async Task<IActionResult> Create([FromBody] LoHangCreateDto dto)
        {
            if (!User.DuocPhepThaoTacChiNhanh(dto.ChiNhanhId))
                return Forbid();

            var (success, errorMessage, result) = await _service.CreateAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.LoHangId }, result);
        }

        // Xả kho — CHỈ Quản lý cửa hàng / Admin (quyết định kinh doanh)
        [HttpPut("{id}/xakho")]
        [Authorize(Roles = "Admin,ChuCuaHang")]
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
        [Authorize(Roles = "Admin,ChuCuaHang")]
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

        // MỚI: Nhân viên điều chỉnh tồn kho vận hành hằng ngày (xuất kho giao hàng, kiểm kê)
        [HttpPut("{id}/dieuchinhton")]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> DieuChinhTonKho(int id, [FromBody] DieuChinhTonKhoDto dto)
        {
            var loHang = await _service.GetByIdAsync(id);
            if (loHang == null) return NotFound(new { message = "Không tìm thấy lô hàng." });

            if (!User.DuocPhepThaoTacChiNhanh(loHang.ChiNhanhId))
                return Forbid();

            var (success, errorMessage) = await _service.DieuChinhTonKhoAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã điều chỉnh tồn kho." });
        }
    }
}