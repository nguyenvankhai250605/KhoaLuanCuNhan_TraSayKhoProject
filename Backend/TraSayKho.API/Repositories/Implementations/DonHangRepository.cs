using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly TraSayKhoDbContext _context;

        public DonHangRepository(TraSayKhoDbContext context)
        {
            _context = context;
        }

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
            return await _context.DonHangs
                .FirstOrDefaultAsync(dh => dh.DonHangId == id);
        }

        public async Task<TrangThaiDonHang?> GetTrangThaiByTenAsync(
            string tenTrangThai)
        {
            return await _context.TrangThaiDonHangs
                .FirstOrDefaultAsync(tt =>
                    tt.TenTrangThai == tenTrangThai);
        }

        public async Task<bool> UpdateTrangThaiAsync(
            int donHangId,
            int trangThaiIdMoi)
        {
            var donHang = await _context.DonHangs.FindAsync(donHangId);

            if (donHang == null)
                return false;

            donHang.TrangThaiId = trangThaiIdMoi;

            _context.LichSuTrangThaiDonHangs.Add(
                new LichSuTrangThaiDonHang
                {
                    DonHangId = donHangId,
                    TrangThaiId = trangThaiIdMoi,
                    ThoiGianCapNhat = DateTime.Now
                });

            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // PHỤC VỤ TẠO ĐƠN HÀNG
        // =========================================================

        public async Task<bool> KhachHangExistsAsync(int khachHangId)
        {
            return await _context.KhachHangs
                .AnyAsync(kh => kh.KhachHangId == khachHangId);
        }

        public async Task<bool> ChiNhanhExistsAsync(int chiNhanhId)
        {
            return await _context.ChiNhanhs
                .AnyAsync(cn => cn.ChiNhanhId == chiNhanhId);
        }

        public async Task<SanPham?> GetSanPhamAsync(int sanPhamId)
        {
            return await _context.SanPhams
                .FirstOrDefaultAsync(sp =>
                    sp.SanPhamId == sanPhamId);
        }

        /// <summary>
        /// Lấy các đơn vị sản phẩm còn trong kho tại đúng chi nhánh.
        /// Các đơn vị được sắp xếp theo nguyên tắc FEFO:
        /// lô có hạn sử dụng gần nhất được bán trước.
        /// </summary>
        public async Task<List<DonViSanPham>>
            GetDonViSanPhamConKhoTheoFefoAsync(
                int sanPhamId,
                int chiNhanhId)
        {
            return await _context.DonViSanPhams
                .Include(dv => dv.Thung)
                    .ThenInclude(t => t.LoHang)
                .Where(dv =>
                    dv.TrangThai == "ConKho" &&
                    dv.Thung.ChiNhanhId == chiNhanhId &&
                    dv.Thung.LoHang.SanPhamId == sanPhamId &&
                    dv.Thung.TrangThai == "ConHang" &&
                    dv.Thung.LoHang.TrangThai == "ConHang")
                .OrderBy(dv => dv.Thung.LoHang.HanSuDung)
                .ThenBy(dv => dv.Thung.NgayPhanBo)
                .ThenBy(dv => dv.DonViId)
                .ToListAsync();
        }

        public async Task<KhuyenMai?> GetKhuyenMaiByMaCodeAsync(
            string maCode)
        {
            return await _context.KhuyenMais
                .FirstOrDefaultAsync(km =>
                    km.MaCode == maCode);
        }

        public async Task<DonHang> TaoDonHangAsync(
            DonHang donHang,
            List<ChiTietDonHang> chiTiets,
            Dictionary<ChiTietDonHang, List<DonViSanPham>>
                danhSachDonViBan,
            KhuyenMai? khuyenMaiApDung)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =================================================
                // 1. LƯU ĐƠN HÀNG
                // =================================================
                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                // =================================================
                // 2. LƯU CHI TIẾT ĐƠN HÀNG
                // =================================================
                foreach (var chiTiet in chiTiets)
                {
                    chiTiet.DonHangId = donHang.DonHangId;
                    _context.ChiTietDonHangs.Add(chiTiet);
                }

                // Lưu để lấy ChiTietDonHangId.
                await _context.SaveChangesAsync();

                // =================================================
                // 3. ĐÁNH DẤU CÁC ĐƠN VỊ SẢN PHẨM ĐÃ BÁN
                // =================================================
                foreach (var item in danhSachDonViBan)
                {
                    var chiTietDonHang = item.Key;
                    var cacDonVi = item.Value;

                    foreach (var donVi in cacDonVi)
                    {
                        donVi.TrangThai = "DaBan";
                        donVi.ChiTietDonHangId =
                            chiTietDonHang.ChiTietDonHangId;
                        donVi.NgayBan = DateTime.Now;
                    }
                }

                // Phải lưu trạng thái DaBan trước khi kiểm tra
                // thùng và lô còn đơn vị nào trong kho hay không.
                await _context.SaveChangesAsync();

                // =================================================
                // 4. CẬP NHẬT TRẠNG THÁI THÙNG
                // =================================================
                var cacThungBiAnhHuong = danhSachDonViBan
                    .SelectMany(item => item.Value)
                    .Select(dv => dv.Thung)
                    .DistinctBy(thung => thung.ThungId)
                    .ToList();

                foreach (var thung in cacThungBiAnhHuong)
                {
                    var conDonViTrongKho =
                        await _context.DonViSanPhams
                            .AnyAsync(dv =>
                                dv.ThungId == thung.ThungId &&
                                dv.TrangThai == "ConKho");

                    if (!conDonViTrongKho)
                    {
                        thung.TrangThai = "HetHang";
                    }
                }

                // =================================================
                // 5. CẬP NHẬT TRẠNG THÁI LÔ HÀNG
                // =================================================
                var cacLoBiAnhHuong = cacThungBiAnhHuong
                    .Select(thung => thung.LoHang)
                    .DistinctBy(lo => lo.LoHangId)
                    .ToList();

                foreach (var loHang in cacLoBiAnhHuong)
                {
                    var conDonViTrongLo =
                        await _context.DonViSanPhams
                            .AnyAsync(dv =>
                                dv.Thung.LoHangId == loHang.LoHangId &&
                                dv.TrangThai == "ConKho");

                    if (!conDonViTrongLo)
                    {
                        loHang.TrangThai = "HetHang";
                    }
                }

                // =================================================
                // 6. TĂNG LƯỢT SỬ DỤNG KHUYẾN MÃI
                // =================================================
                if (khuyenMaiApDung != null)
                {
                    khuyenMaiApDung.SoLuotDaSuDung += 1;
                }

                // =================================================
                // 7. LƯU LỊCH SỬ TRẠNG THÁI ĐƠN HÀNG
                // =================================================
                _context.LichSuTrangThaiDonHangs.Add(
                    new LichSuTrangThaiDonHang
                    {
                        DonHangId = donHang.DonHangId,
                        TrangThaiId = donHang.TrangThaiId,
                        ThoiGianCapNhat = DateTime.Now
                    });

                await _context.SaveChangesAsync();

                // =================================================
                // 8. ĐỒNG BỘ SỐ LƯỢNG TỒN SẢN PHẨM
                // =================================================
                var cacSanPhamBiAnhHuong = chiTiets
                    .Select(ct => ct.SanPhamId)
                    .Distinct()
                    .ToList();

                foreach (var sanPhamId in cacSanPhamBiAnhHuong)
                {
                    var sanPham =
                        await _context.SanPhams.FindAsync(sanPhamId);

                    if (sanPham == null)
                        continue;

                    sanPham.SoLuongTon =
                        await _context.DonViSanPhams
                            .CountAsync(dv =>
                                dv.Thung.LoHang.SanPhamId ==
                                    sanPhamId &&
                                dv.TrangThai == "ConKho");
                }

                await _context.SaveChangesAsync();

                // =================================================
                // 9. XÓA SẢN PHẨM ĐÃ ĐẶT KHỎI GIỎ HÀNG
                // =================================================
                var cacSanPhamDaDat = chiTiets
                    .Select(ct => ct.SanPhamId)
                    .Distinct()
                    .ToList();

                var gioHang = await _context.GioHangs
                    .Include(gh => gh.ChiTietGioHangs)
                    .FirstOrDefaultAsync(gh =>
                        gh.KhachHangId == donHang.KhachHangId);

                if (gioHang != null)
                {
                    var cacChiTietCanXoa =
                        gioHang.ChiTietGioHangs
                            .Where(ct =>
                                cacSanPhamDaDat.Contains(
                                    ct.SanPhamId))
                            .ToList();

                    if (cacChiTietCanXoa.Count > 0)
                    {
                        _context.ChiTietGioHangs.RemoveRange(
                            cacChiTietCanXoa);

                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                return donHang;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}