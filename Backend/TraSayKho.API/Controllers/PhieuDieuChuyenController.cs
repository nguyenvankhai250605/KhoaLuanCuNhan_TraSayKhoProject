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
    public class PhieuDieuChuyenController : ControllerBase
    {
        private readonly IPhieuDieuChuyenService _service;
        public PhieuDieuChuyenController(IPhieuDieuChuyenService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();

            if (!User.CoQuyenXemToanHeThong())
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

            if (!User.CoQuyenXemToanHeThong())
            {
                var chiNhanhId = User.GetChiNhanhId();
                if (result.ChiNhanhGuiId != chiNhanhId && result.ChiNhanhNhanId != chiNhanhId)
                    return Forbid();
            }

            return Ok(result);
        }

        // BƯỚC 1: Chủ cửa hàng chi nhánh THIẾU HÀNG tạo yêu cầu
        [HttpPost]
        [Authorize(Roles = "Admin,ChuCuaHang")]
        public async Task<IActionResult> TaoYeuCau([FromBody] PhieuDieuChuyenCreateDto dto)
        {
            // Người tạo phải thuộc đúng chi nhánh ĐANG THIẾU HÀNG (chi nhánh nhận)
            if (!User.DuocPhepThaoTacChiNhanh(dto.ChiNhanhNhanId))
                return Forbid();

            var (success, errorMessage, result) = await _service.TaoYeuCauAsync(dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return CreatedAtAction(nameof(GetById), new { id = result!.PhieuDieuChuyenId }, result);
        }

        // BƯỚC 2a: Chủ cửa hàng chi nhánh NGUỒN duyệt
        [HttpPut("{id}/duyet")]
        [Authorize(Roles = "Admin,ChuCuaHang")]
        public async Task<IActionResult> Duyet(int id)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            if (!User.DuocPhepThaoTacChiNhanh(phieu.ChiNhanhGuiId))
                return Forbid();

            var (success, errorMessage) = await _service.DuyetAsync(id);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã duyệt yêu cầu điều chuyển." });
        }

        // BƯỚC 2b: Chủ cửa hàng chi nhánh NGUỒN từ chối
        [HttpPut("{id}/tuchoi")]
        [Authorize(Roles = "Admin,ChuCuaHang")]
        public async Task<IActionResult> TuChoi(int id, [FromBody] TuChoiPhieuDto dto)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            if (!User.DuocPhepThaoTacChiNhanh(phieu.ChiNhanhGuiId))
                return Forbid();

            var (success, errorMessage) = await _service.TuChoiAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã từ chối yêu cầu điều chuyển." });
        }

        // BƯỚC 3: Nhân viên chi nhánh NGUỒN xác nhận đã xuất kho
        [HttpPut("{id}/xuatkho")]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> XacNhanXuatKho(int id, [FromBody] XacNhanThucHienDto dto)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            if (!User.DuocPhepThaoTacChiNhanh(phieu.ChiNhanhGuiId))
                return Forbid();

            var (success, errorMessage) = await _service.XacNhanXuatKhoAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã xuất kho, hàng đang được vận chuyển." });
        }

        // BƯỚC 4: Nhân viên chi nhánh ĐÍCH xác nhận đã nhận hàng
        [HttpPut("{id}/nhanhang")]
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> XacNhanNhanHang(int id, [FromBody] XacNhanThucHienDto dto)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound(new { message = "Không tìm thấy phiếu điều chuyển." });

            if (!User.DuocPhepThaoTacChiNhanh(phieu.ChiNhanhNhanId))
                return Forbid();

            var (success, errorMessage) = await _service.XacNhanNhanHangAsync(id, dto);
            if (!success) return BadRequest(new { message = errorMessage });
            return Ok(new { message = "Đã nhận hàng, hoàn tất điều chuyển kho." });
        }
    }
}