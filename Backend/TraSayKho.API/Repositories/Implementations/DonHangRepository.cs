using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly TraSayKhoDbContext _context;
        public DonHangRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<List<DonHang>> GetAllAsync()
        {
            return await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.TrangThai)
                .Include(dh => dh.ChiNhanh)
                .ToListAsync();
        }

        public async Task<DonHang?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DonHangs
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.TrangThai)
                .Include(dh => dh.ChiNhanh)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(dh => dh.DonHangId == id);
        }

        public async Task<DonHang?> GetByIdAsync(int id)
        {
            return await _context.DonHangs.FirstOrDefaultAsync(dh => dh.DonHangId == id);
        }

        public async Task<TrangThaiDonHang?> GetTrangThaiByTenAsync(string tenTrangThai)
        {
            return await _context.TrangThaiDonHangs
                .FirstOrDefaultAsync(tt => tt.TenTrangThai == tenTrangThai);
        }

        public async Task<bool> UpdateTrangThaiAsync(int donHangId, int trangThaiIdMoi)
        {
            var donHang = await _context.DonHangs.FindAsync(donHangId);
            if (donHang == null) return false;

            donHang.TrangThaiId = trangThaiIdMoi;

            _context.LichSuTrangThaiDonHangs.Add(new LichSuTrangThaiDonHang
            {
                DonHangId = donHangId,
                TrangThaiId = trangThaiIdMoi,
                ThoiGianCapNhat = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // ==== CÁC HÀM MỚI ====

        public async Task<bool> KhachHangExistsAsync(int khachHangId)
        {
            return await _context.KhachHangs.AnyAsync(kh => kh.KhachHangId == khachHangId);
        }

        public async Task<bool> ChiNhanhExistsAsync(int chiNhanhId)
        {
            return await _context.ChiNhanhs.AnyAsync(cn => cn.ChiNhanhId == chiNhanhId);
        }

        public async Task<SanPham?> GetSanPhamAsync(int sanPhamId)
        {
            return await _context.SanPhams.FirstOrDefaultAsync(sp => sp.SanPhamId == sanPhamId);
        }

        public async Task<List<LoHang>> GetLoHangConHangTheoFefoAsync(int sanPhamId, int chiNhanhId)
        {
            return await _context.LoHangs
                .Where(lh => lh.SanPhamId == sanPhamId && lh.ChiNhanhId == chiNhanhId && lh.TrangThai == "ConHang")
                .OrderBy(lh => lh.HanSuDung)   // FEFO: lô hết hạn sớm nhất đứng đầu danh sách
                .ToListAsync();
        }

        public async Task<KhuyenMai?> GetKhuyenMaiByMaCodeAsync(string maCode)
        {
            return await _context.KhuyenMais.FirstOrDefaultAsync(km => km.MaCode == maCode);
        }

        public async Task<DonHang> TaoDonHangAsync(
            DonHang donHang,
            List<ChiTietDonHang> chiTiets,
            List<(LoHang LoHang, int SoLuongTru)> danhSachTruKho,
            KhuyenMai? khuyenMaiApDung)
        {
            // 1. Lưu đơn hàng trước để có DonHangId
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // 2. Lưu chi tiết đơn hàng, gắn đúng DonHangId vừa tạo
            foreach (var ct in chiTiets)
            {
                ct.DonHangId = donHang.DonHangId;
                _context.ChiTietDonHangs.Add(ct);
            }

            // 3. Trừ kho ở đúng lô đã chọn cho từng dòng
            foreach (var (loHang, soLuongTru) in danhSachTruKho)
            {
                loHang.SoLuongConLai -= soLuongTru;
                if (loHang.SoLuongConLai <= 0)
                    loHang.TrangThai = "HetHang";
            }

            // 4. Nếu có dùng mã khuyến mãi, tăng số lượt đã sử dụng
            if (khuyenMaiApDung != null)
                khuyenMaiApDung.SoLuotDaSuDung += 1;

            // 5. Ghi lịch sử trạng thái đầu tiên (ChoXacNhan)
            _context.LichSuTrangThaiDonHangs.Add(new LichSuTrangThaiDonHang
            {
                DonHangId = donHang.DonHangId,
                TrangThaiId = donHang.TrangThaiId,
                ThoiGianCapNhat = DateTime.Now
            });

            // 6. Lưu tất cả thay đổi ở bước 2-5 cùng lúc (an toàn — nếu lỗi giữa chừng, mọi thứ được hủy đồng thời)
            await _context.SaveChangesAsync();

            // 7. Đồng bộ lại tồn kho tổng của từng sản phẩm bị ảnh hưởng
            var cacSanPhamBiAnhHuong = danhSachTruKho.Select(x => x.LoHang.SanPhamId).Distinct();
            foreach (var sanPhamId in cacSanPhamBiAnhHuong)
            {
                var sanPham = await _context.SanPhams.FindAsync(sanPhamId);
                if (sanPham == null) continue;

                var loConHang = await _context.LoHangs
                    .Where(lh => lh.SanPhamId == sanPhamId && lh.TrangThai == "ConHang")
                    .ToListAsync();

                sanPham.SoLuongTon = loConHang.Sum(lh => lh.SoLuongConLai);
                sanPham.HanSuDung = loConHang.Any() ? loConHang.Min(lh => lh.HanSuDung) : null;
            }
            await _context.SaveChangesAsync();

            return donHang;
        }
    }
}