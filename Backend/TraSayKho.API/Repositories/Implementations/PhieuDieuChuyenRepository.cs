using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class PhieuDieuChuyenRepository
        : IPhieuDieuChuyenRepository
    {
        private readonly TraSayKhoDbContext
            _context;

        public PhieuDieuChuyenRepository(
            TraSayKhoDbContext context)
        {
            _context = context;
        }

        public async Task<bool>
            ChiNhanhExistsAsync(int chiNhanhId)
        {
            return await _context.ChiNhanhs
                .AnyAsync(cn =>
                    cn.ChiNhanhId == chiNhanhId &&
                    cn.TrangThai);
        }

        public async Task<bool>
            NhanVienExistsAsync(int nhanVienId)
        {
            return await _context.NhanViens
                .AnyAsync(nv =>
                    nv.NhanVienId == nhanVienId);
        }

        // Lấy các thùng còn đơn vị sản phẩm
        // và sắp xếp theo FEFO.
        public async Task<List<ThungHang>>
            GetThungHangTheoFefoAsync(
                int sanPhamId,
                int chiNhanhId)
        {
            return await _context.ThungHangs

                .Include(t => t.LoHang)

                .Include(t =>
                    t.DonViSanPhams)

                .Where(t =>
                    t.LoHang.SanPhamId ==
                        sanPhamId &&

                    t.ChiNhanhId ==
                        chiNhanhId &&

                    t.TrangThai ==
                        "ConHang" &&

                    t.LoHang.TrangThai ==
                        "ConHang" &&

                    t.DonViSanPhams.Any(dv =>
                        dv.TrangThai ==
                            "ConKho"))

                .OrderBy(t =>
                    t.LoHang.HanSuDung)

                .ThenBy(t =>
                    t.NgayPhanBo)

                .ThenBy(t =>
                    t.ThungId)

                .ToListAsync();
        }

        public async Task<
            List<PhieuDieuChuyenKho>>
            GetAllAsync()
        {
            return await _context
                .PhieuDieuChuyenKhos

                .Include(p => p.ChiNhanhGui)
                .Include(p => p.ChiNhanhNhan)

                .Include(p => p.NhanVienTao)
                .Include(p => p.NhanVienXacNhan)

                .Include(p =>
                    p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct =>
                        ct.Thung)
                    .ThenInclude(t =>
                        t.LoHang)
                    .ThenInclude(lh =>
                        lh.SanPham)

                .Include(p =>
                    p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct =>
                        ct.Thung)
                    .ThenInclude(t =>
                        t.DonViSanPhams)

                .OrderByDescending(p =>
                    p.NgayTao)

                .ToListAsync();
        }

        public async Task<PhieuDieuChuyenKho?>
            GetByIdAsync(int id)
        {
            return await _context
                .PhieuDieuChuyenKhos

                .Include(p => p.ChiNhanhGui)
                .Include(p => p.ChiNhanhNhan)

                .Include(p => p.NhanVienTao)
                .Include(p => p.NhanVienXacNhan)

                .Include(p =>
                    p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct =>
                        ct.Thung)
                    .ThenInclude(t =>
                        t.LoHang)
                    .ThenInclude(lh =>
                        lh.SanPham)

                .Include(p =>
                    p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct =>
                        ct.Thung)
                    .ThenInclude(t =>
                        t.DonViSanPhams)

                .FirstOrDefaultAsync(p =>
                    p.PhieuDieuChuyenId == id);
        }

        public async Task<PhieuDieuChuyenKho>
            TaoYeuCauAsync(
                PhieuDieuChuyenKho phieu,
                List<ChiTietPhieuDieuChuyen>
                    chiTiets)
        {
            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                _context.PhieuDieuChuyenKhos
                    .Add(phieu);

                await _context.SaveChangesAsync();

                foreach (var chiTiet
                         in chiTiets)
                {
                    chiTiet.PhieuDieuChuyenId =
                        phieu.PhieuDieuChuyenId;

                    _context
                        .ChiTietPhieuDieuChuyens
                        .Add(chiTiet);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (await GetByIdAsync(
                    phieu.PhieuDieuChuyenId))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool>
            DuyetAsync(int phieuId)
        {
            var phieu =
                await _context
                    .PhieuDieuChuyenKhos
                    .FindAsync(phieuId);

            if (phieu == null ||
                phieu.TrangThai != "ChoDuyet")
            {
                return false;
            }

            phieu.TrangThai = "DaDuyet";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool>
            TuChoiAsync(
                int phieuId,
                string lyDo)
        {
            var phieu =
                await _context
                    .PhieuDieuChuyenKhos
                    .FindAsync(phieuId);

            if (phieu == null ||
                phieu.TrangThai != "ChoDuyet")
            {
                return false;
            }

            phieu.TrangThai = "TuChoi";

            phieu.GhiChu =
                $"{phieu.GhiChu} | Lý do từ chối: {lyDo}"
                    .Trim(' ', '|');

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool>
            XacNhanXuatKhoAsync(
                int phieuId,
                int nhanVienId)
        {
            var phieu =
                await _context
                    .PhieuDieuChuyenKhos

                    .Include(p =>
                        p.ChiTietPhieuDieuChuyens)
                        .ThenInclude(ct =>
                            ct.Thung)
                        .ThenInclude(t =>
                            t.DonViSanPhams)

                    .FirstOrDefaultAsync(p =>
                        p.PhieuDieuChuyenId ==
                        phieuId);

            if (phieu == null ||
                phieu.TrangThai != "DaDuyet")
            {
                return false;
            }

            foreach (var chiTiet
                     in phieu
                        .ChiTietPhieuDieuChuyens)
            {
                var thung =
                    chiTiet.Thung;

                if (thung.ChiNhanhId !=
                    phieu.ChiNhanhGuiId)
                {
                    throw new InvalidOperationException(
                        $"Thùng '{thung.MaThung}' không còn thuộc chi nhánh nguồn.");
                }

                var conDonViTrongKho =
                    thung.DonViSanPhams
                        .Any(dv =>
                            dv.TrangThai ==
                                "ConKho");

                if (!conDonViTrongKho)
                {
                    throw new InvalidOperationException(
                        $"Thùng '{thung.MaThung}' không còn đơn vị sản phẩm để điều chuyển.");
                }

                thung.TrangThai =
                    "DangVanChuyen";
            }

            phieu.TrangThai =
                "DangVanChuyen";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool>
            XacNhanNhanHangAsync(
                int phieuId,
                int nhanVienId)
        {
            var phieu =
                await _context
                    .PhieuDieuChuyenKhos

                    .Include(p =>
                        p.ChiTietPhieuDieuChuyens)
                        .ThenInclude(ct =>
                            ct.Thung)
                        .ThenInclude(t =>
                            t.LoHang)

                    .Include(p =>
                        p.ChiTietPhieuDieuChuyens)
                        .ThenInclude(ct =>
                            ct.Thung)
                        .ThenInclude(t =>
                            t.DonViSanPhams)

                    .FirstOrDefaultAsync(p =>
                        p.PhieuDieuChuyenId ==
                        phieuId);

            if (phieu == null ||
                phieu.TrangThai !=
                    "DangVanChuyen")
            {
                return false;
            }

            foreach (var chiTiet
                     in phieu
                        .ChiTietPhieuDieuChuyens)
            {
                var thung =
                    chiTiet.Thung;

                thung.ChiNhanhId =
                    phieu.ChiNhanhNhanId;

                thung.NgayPhanBo =
                    DateOnly.FromDateTime(
                        DateTime.Now);

                var conDonViTrongKho =
                    thung.DonViSanPhams
                        .Any(dv =>
                            dv.TrangThai ==
                                "ConKho");

                thung.TrangThai =
                    conDonViTrongKho
                        ? "ConHang"
                        : "HetHang";
            }

            phieu.TrangThai = "HoanTat";

            phieu.NhanVienXacNhanId =
                nhanVienId;

            phieu.NgayXacNhan =
                DateTime.Now;

            await _context.SaveChangesAsync();

            var sanPhamIds =
                phieu
                    .ChiTietPhieuDieuChuyens
                    .Select(ct =>
                        ct.Thung.LoHang
                            .SanPhamId)
                    .Distinct()
                    .ToList();

            foreach (var sanPhamId
                     in sanPhamIds)
            {
                await DongBoTonKhoSanPhamAsync(
                    sanPhamId);
            }

            return true;
        }

        public async Task
            DongBoTonKhoSanPhamAsync(
                int sanPhamId)
        {
            var sanPham =
                await _context.SanPhams
                    .FindAsync(sanPhamId);

            if (sanPham == null)
                return;

            var tongDonViConKho =
                await _context
                    .DonViSanPhams
                    .CountAsync(dv =>
                        dv.Thung.LoHang
                            .SanPhamId ==
                            sanPhamId &&

                        dv.TrangThai ==
                            "ConKho");

            sanPham.SoLuongTon =
                tongDonViConKho;

            await _context.SaveChangesAsync();
        }
    }
}