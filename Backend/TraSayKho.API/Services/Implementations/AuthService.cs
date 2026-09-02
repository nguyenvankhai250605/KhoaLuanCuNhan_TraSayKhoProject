using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<(bool Success, string? ErrorMessage)> DangKyKhachHangAsync(DangKyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenDangNhap) || string.IsNullOrWhiteSpace(dto.MatKhau))
                return (false, "Tên đăng nhập và mật khẩu không được để trống.");

            if (dto.MatKhau.Length < 6)
                return (false, "Mật khẩu phải có ít nhất 6 ký tự.");

            if (await _repository.TenDangNhapExistsAsync(dto.TenDangNhap))
                return (false, "Tên đăng nhập đã tồn tại.");

            if (await _repository.EmailExistsAsync(dto.Email))
                return (false, "Email đã được sử dụng.");

            var vaiTroId = await _repository.GetVaiTroIdAsync("KhachHang");
            if (vaiTroId == null)
                return (false, "Không tìm thấy vai trò Khách hàng trong hệ thống.");

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = dto.TenDangNhap,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
                Email = dto.Email,
                SoDienThoai = dto.SoDienThoai,
                VaiTroId = vaiTroId.Value,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            var khachHang = new KhachHang
            {
                HoTen = dto.HoTen,
                DiaChi = dto.DiaChi
            };

            await _repository.DangKyKhachHangAsync(taiKhoan, khachHang);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> TaoNhanVienAsync(TaoTaiKhoanNhanVienDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenDangNhap) || string.IsNullOrWhiteSpace(dto.MatKhau))
                return (false, "Tên đăng nhập và mật khẩu không được để trống.");

            if (dto.MatKhau.Length < 6)
                return (false, "Mật khẩu phải có ít nhất 6 ký tự.");

            if (await _repository.TenDangNhapExistsAsync(dto.TenDangNhap))
                return (false, "Tên đăng nhập đã tồn tại.");

            if (await _repository.EmailExistsAsync(dto.Email))
                return (false, "Email đã được sử dụng.");

            if (dto.TenVaiTro != "Admin" && dto.TenVaiTro != "NhanVien")
                return (false, "Vai trò không hợp lệ (chỉ chấp nhận Admin hoặc NhanVien).");

            if (dto.ChiNhanhId.HasValue && !await _repository.ChiNhanhExistsAsync(dto.ChiNhanhId.Value))
                return (false, "Chi nhánh không tồn tại.");

            var vaiTroId = await _repository.GetVaiTroIdAsync(dto.TenVaiTro);
            if (vaiTroId == null)
                return (false, "Không tìm thấy vai trò trong hệ thống.");

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = dto.TenDangNhap,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
                Email = dto.Email,
                SoDienThoai = dto.SoDienThoai,
                VaiTroId = vaiTroId.Value,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            var nhanVien = new NhanVien
            {
                HoTen = dto.HoTen,
                ChucVu = dto.ChucVu,
                ChiNhanhId = dto.ChiNhanhId,
                NgayVaoLam = DateOnly.FromDateTime(DateTime.Now)
            };

            await _repository.TaoNhanVienAsync(taiKhoan, nhanVien);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, DangNhapResponseDto? Result)> DangNhapAsync(DangNhapDto dto)
        {
            var taiKhoan = await _repository.GetTaiKhoanDayDuAsync(dto.TenDangNhap);

            if (taiKhoan == null)
                return (false, "Tên đăng nhập hoặc mật khẩu không đúng.", null);

            if (!taiKhoan.TrangThai)
                return (false, "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.", null);

            bool matKhauDung;
            try
            {
                matKhauDung = BCrypt.Net.BCrypt.Verify(dto.MatKhau, taiKhoan.MatKhauHash);
            }
            catch
            {
                matKhauDung = false;   // dữ liệu mẫu cũ chưa hash đúng chuẩn BCrypt sẽ rơi vào đây
            }

            if (!matKhauDung)
                return (false, "Tên đăng nhập hoặc mật khẩu không đúng.", null);

            var hoTen = taiKhoan.NhanVien?.HoTen ?? taiKhoan.KhachHang?.HoTen ?? taiKhoan.TenDangNhap;
            var chiNhanhId = taiKhoan.NhanVien?.ChiNhanhId;
            var tenChiNhanh = taiKhoan.NhanVien?.ChiNhanh?.TenChiNhanh;

            var (token, thoiGianHetHan) = TaoJwtToken(taiKhoan, hoTen, chiNhanhId);

            return (true, null, new DangNhapResponseDto
            {
                Token = token,
                TenDangNhap = taiKhoan.TenDangNhap,
                HoTen = hoTen,
                VaiTro = taiKhoan.VaiTro.TenVaiTro,
                ChiNhanhId = chiNhanhId,
                TenChiNhanh = tenChiNhanh,
                ThoiGianHetHan = thoiGianHetHan
            });
        }

        private (string Token, DateTime ThoiGianHetHan) TaoJwtToken(TaiKhoan taiKhoan, string hoTen, int? chiNhanhId)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"]!;
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "1440");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.TaiKhoanId.ToString()),
                new Claim(ClaimTypes.Name, taiKhoan.TenDangNhap),
                new Claim(ClaimTypes.Role, taiKhoan.VaiTro.TenVaiTro),
                new Claim("HoTen", hoTen)
            };

            if (chiNhanhId.HasValue)
                claims.Add(new Claim("ChiNhanhId", chiNhanhId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var thoiGianHetHan = DateTime.Now.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: thoiGianHetHan,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), thoiGianHetHan);
        }
    }
}