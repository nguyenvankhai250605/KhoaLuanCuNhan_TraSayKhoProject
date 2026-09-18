using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class LoHangRepository : ILoHangRepository
    {
        private readonly TraSayKhoDbContext _context;

        public LoHangRepository(
            TraSayKhoDbContext context)
        {
            _context = context;
        }

        public async Task<bool>
            SanPhamExistsAsync(int sanPhamId)
        {
            return await _context.SanPhams
                .AnyAsync(sp =>
                    sp.SanPhamId == sanPhamId);
        }

        public async Task<bool>
            SoLoExistsAsync(string soLo)
        {
            return await _context.LoHangs
                .AnyAsync(lh =>
                    lh.SoLo == soLo);
        }

        public async Task<List<LoHang>>
            GetAllAsync()
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t => t.ChiNhanh)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t =>
                        t.DonViSanPhams)
                .OrderBy(lh => lh.HanSuDung)
                .ToListAsync();
        }

        public async Task<LoHang?>
            GetByIdAsync(int id)
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t => t.ChiNhanh)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t =>
                        t.DonViSanPhams)
                .FirstOrDefaultAsync(lh =>
                    lh.LoHangId == id);
        }

        public async Task<List<LoHang>>
            GetBySanPhamAsync(int sanPhamId)
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t => t.ChiNhanh)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t =>
                        t.DonViSanPhams)
                .Where(lh =>
                    lh.SanPhamId == sanPhamId)
                .OrderBy(lh => lh.HanSuDung)
                .ToListAsync();
        }

        public async Task<List<LoHang>>
            GetSapHetHanAsync()
        {
            return await _context.LoHangs
                .Include(lh => lh.SanPham)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t => t.ChiNhanh)
                .Include(lh => lh.ThungHangs)
                    .ThenInclude(t =>
                        t.DonViSanPhams)
                .Where(lh =>
                    lh.TrangThai == "ConHang" &&
                    lh.ThungHangs.Any(t =>
                        t.DonViSanPhams.Any(dv =>
                            dv.TrangThai == "ConKho")))
                .OrderBy(lh => lh.HanSuDung)
                .ToListAsync();
        }

        public async Task<LoHang>
            TaoLoVaPhanBoAsync(
                LoHang loHang,
                int chiNhanhChinhId,
                int soLuongThung,
                int soDonViMoiThung)
        {
            var sanPham = await _context.SanPhams
                .AsNoTracking()
                .FirstOrDefaultAsync(sp =>
                    sp.SanPhamId ==
                    loHang.SanPhamId);

            if (sanPham == null)
            {
                throw new InvalidOperationException(
                    "Sản phẩm không tồn tại.");
            }

            string tienToDonVi;

            if (sanPham.DonViTinh == "Hộp")
            {
                tienToDonVi = "HOP";
            }
            else if (sanPham.DonViTinh == "Gói")
            {
                tienToDonVi = "GOI";
            }
            else
            {
                throw new InvalidOperationException(
                    "Đơn vị tính của sản phẩm phải là Hộp hoặc Gói.");
            }

            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                _context.LoHangs.Add(loHang);
                await _context.SaveChangesAsync();

                var soDonViDaTao = 0;

                for (var i = 1;
                     i <= soLuongThung;
                     i++)
                {
                    var thung = new ThungHang
                    {
                        LoHangId =
                            loHang.LoHangId,

                        ChiNhanhId =
                            chiNhanhChinhId,

                        MaThung =
                            $"{loHang.SoLo}-T{i:D3}",

                        SoLuongDonVi =
                            soDonViMoiThung,

                        NgayPhanBo =
                            DateOnly.FromDateTime(
                                DateTime.Now),

                        TrangThai =
                            "ConHang"
                    };

                    _context.ThungHangs.Add(thung);
                    await _context.SaveChangesAsync();

                    var danhSachDonVi =
                        new List<DonViSanPham>();

                    for (var j = 1;
                         j <= soDonViMoiThung;
                         j++)
                    {
                        soDonViDaTao++;

                        danhSachDonVi.Add(
                            new DonViSanPham
                            {
                                ThungId =
                                    thung.ThungId,

                                MaDonVi =
                                    $"{tienToDonVi}-{loHang.SoLo}-{soDonViDaTao:D6}",

                                TrangThai =
                                    "ConKho",

                                ChiTietDonHangId =
                                    null,

                                NgayBan =
                                    null
                            });
                    }

                    _context.DonViSanPhams
                        .AddRange(danhSachDonVi);

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return (await GetByIdAsync(
                    loHang.LoHangId))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

            var soLuongConKho =
                await _context.DonViSanPhams
                    .CountAsync(dv =>
                        dv.Thung.LoHang.SanPhamId ==
                            sanPhamId &&
                        dv.TrangThai ==
                            "ConKho");

            sanPham.SoLuongTon =
                soLuongConKho;

            await _context.SaveChangesAsync();
        }

        public async Task<DonViSanPham?>
            GetDonViSanPhamByMaAsync(
                string maDonVi)
        {
            return await _context
                .DonViSanPhams

                .Include(dv => dv.Thung)
                    .ThenInclude(t =>
                        t.ChiNhanh)

                .Include(dv => dv.Thung)
                    .ThenInclude(t =>
                        t.LoHang)
                    .ThenInclude(lh =>
                        lh.SanPham)

                .Include(dv =>
                    dv.ChiTietDonHang)
                    .ThenInclude(ct =>
                        ct!.DonHang)
                    .ThenInclude(dh =>
                        dh.KhachHang)

                .Include(dv =>
                    dv.ChiTietDonHang)
                    .ThenInclude(ct =>
                        ct!.DonHang)
                    .ThenInclude(dh =>
                        dh.ChiNhanh)

                .FirstOrDefaultAsync(dv =>
                    dv.MaDonVi == maDonVi);
        }

        public async Task<ChiNhanh?>
            GetTruSoChinhAsync()
        {
            return await _context.ChiNhanhs
                .FirstOrDefaultAsync(cn =>
                    cn.LaTruSoChinh &&
                    cn.TrangThai);
        }
    }
}