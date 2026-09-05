using System.Security.Claims;

namespace TraSayKho.API.Helpers
{
    public static class ClaimsExtensions
    {
        // Lấy ChiNhanhId từ token, null nghĩa là Admin tổng
        public static int? GetChiNhanhId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("ChiNhanhId")?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }

        public static bool LaAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        // Kiểm tra: người dùng có được phép thao tác dữ liệu của chiNhanhId này không
        public static bool DuocPhepThaoTacChiNhanh(this ClaimsPrincipal user, int chiNhanhId)
        {
            if (user.LaAdmin()) return true;   // Admin luôn được phép

            var chiNhanhCuaToi = user.GetChiNhanhId();
            return chiNhanhCuaToi.HasValue && chiNhanhCuaToi.Value == chiNhanhId;
        }
    }
}