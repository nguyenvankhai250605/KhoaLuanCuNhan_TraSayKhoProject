using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraSayKho.API.DTOs;
using TraSayKho.API.Helpers;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(
        Roles = "Admin,NhanVien,ChuCuaHang")]
    public class LoHangController : ControllerBase
    {
        private readonly ILoHangService _service;

        public LoHangController(
            ILoHangService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list =
                await _service.GetAllAsync();

            if (!User.CoQuyenXemToanHeThong())
            {
                var chiNhanhId =
                    User.GetChiNhanhId();

                if (!chiNhanhId.HasValue)
                    return Forbid();

                list = LocDanhSachTheoChiNhanh(
                    list,
                    chiNhanhId.Value);
            }

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            var result =
                await _service
                    .GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message =
                        "Không tìm thấy lô hàng."
                });
            }

            if (!User.CoQuyenXemToanHeThong())
            {
                var chiNhanhId =
                    User.GetChiNhanhId();

                if (!chiNhanhId.HasValue)
                    return Forbid();

                result.DanhSachThung =
                    result.DanhSachThung
                        .Where(t =>
                            t.ChiNhanhId ==
                            chiNhanhId.Value)
                        .ToList();

                if (result.DanhSachThung.Count == 0)
                    return Forbid();

                result.TongSoLuongConKho =
                    result.DanhSachThung
                        .Sum(t =>
                            t.SoLuongConKho);
            }

            return Ok(result);
        }

        [HttpGet(
            "sanpham/{sanPhamId:int}")]
        public async Task<IActionResult>
            GetBySanPham(int sanPhamId)
        {
            var list =
                await _service
                    .GetBySanPhamAsync(
                        sanPhamId);

            if (!User.CoQuyenXemToanHeThong())
            {
                var chiNhanhId =
                    User.GetChiNhanhId();

                if (!chiNhanhId.HasValue)
                    return Forbid();

                list = LocDanhSachTheoChiNhanh(
                    list,
                    chiNhanhId.Value);
            }

            return Ok(list);
        }

        [HttpGet("saphethan")]
        public async Task<IActionResult>
            GetSapHetHan(
                [FromQuery] int phanTram = 25)
        {
            var list =
                await _service
                    .GetSapHetHanAsync(
                        phanTram);

            if (!User.CoQuyenXemToanHeThong())
            {
                var chiNhanhId =
                    User.GetChiNhanhId();

                if (!chiNhanhId.HasValue)
                    return Forbid();

                list = LocDanhSachTheoChiNhanh(
                    list,
                    chiNhanhId.Value);
            }

            return Ok(list);
        }

        // Chỉ Admin hoặc Chủ cửa hàng thuộc
        // chi nhánh chính được nhập lô mới.
        [HttpPost]
        [Authorize(
            Roles = "Admin,ChuCuaHang")]
        public async Task<IActionResult> Create(
            [FromBody] LoHangCreateDto dto)
        {
            int? chiNhanhNguoiGoi = null;

            if (!User.LaAdmin())
            {
                var chiNhanhId =
                    User.GetChiNhanhId();

                if (!chiNhanhId.HasValue)
                    return Forbid();

                chiNhanhNguoiGoi =
                    chiNhanhId.Value;
            }

            var (
                success,
                errorMessage,
                result
            ) =
                await _service.CreateAsync(
                    dto,
                    chiNhanhNguoiGoi);

            if (!success)
            {
                return BadRequest(new
                {
                    message = errorMessage
                });
            }

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = result!.LoHangId
                },
                result);
        }

        // Khách hàng có thể truy xuất nguồn gốc,
        // nhưng thông tin người mua chỉ được trả về
        // cho nhân viên có quyền quản lý.
        [HttpGet("truyxuat/{maDonVi}")]
        [AllowAnonymous]
        public async Task<IActionResult>
            TruyXuat(string maDonVi)
        {
            var (
                success,
                errorMessage,
                result
            ) =
                await _service
                    .TruyXuatTheoMaDonViAsync(
                        maDonVi);

            if (!success)
            {
                return NotFound(new
                {
                    message = errorMessage
                });
            }

            var laNguoiQuanLy =
                User.Identity?.IsAuthenticated ==
                    true &&
                (
                    User.IsInRole("Admin") ||
                    User.IsInRole("NhanVien") ||
                    User.IsInRole("ChuCuaHang")
                );

            if (laNguoiQuanLy)
                return Ok(result);

            // Bản công khai không trả thông tin
            // khách hàng và mã đơn hàng.
            return Ok(new
            {
                result!.DonViId,
                result.MaDonVi,
                result.DonViTinh,
                result.TrangThaiDonVi,
                result.MaThung,
                result.ChiNhanhPhanBoId,
                result.TenChiNhanhPhanBo,
                result.SoLo,
                result.NgaySanXuat,
                result.HanSuDung,
                result.SanPhamId,
                result.TenSanPham
            });
        }

        private static List<LoHangDto>
            LocDanhSachTheoChiNhanh(
                IEnumerable<LoHangDto> danhSach,
                int chiNhanhId)
        {
            return danhSach
                .Select(lh =>
                {
                    lh.DanhSachThung =
                        lh.DanhSachThung
                            .Where(t =>
                                t.ChiNhanhId ==
                                chiNhanhId)
                            .ToList();

                    lh.TongSoLuongConKho =
                        lh.DanhSachThung
                            .Sum(t =>
                                t.SoLuongConKho);

                    return lh;
                })
                .Where(lh =>
                    lh.DanhSachThung.Count > 0)
                .ToList();
        }
    }
}