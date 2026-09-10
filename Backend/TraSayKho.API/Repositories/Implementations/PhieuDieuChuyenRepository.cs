using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class PhieuDieuChuyenRepository : IPhieuDieuChuyenRepository
    {
        private readonly TraSayKhoDbContext _context;
        public PhieuDieuChuyenRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> ChiNhanhExistsAsync(int chiNhanhId)
        {
            return await _context.ChiNhanhs.AnyAsync(cn => cn.ChiNhanhId == chiNhanhId);
        }

        public async Task<bool> NhanVienExistsAsync(int nhanVienId)
        {
            return await _context.NhanViens.AnyAsync(nv => nv.NhanVienId == nhanVienId);
        }

        public async Task<List<LoHang>> GetLoHangConHangTheoFefoAsync(int sanPhamId, int chiNhanhId)
        {
            return await _context.LoHangs
                .Where(lh => lh.SanPhamId == sanPhamId && lh.ChiNhanhId == chiNhanhId && lh.TrangThai == "ConHang")
                .OrderBy(lh => lh.HanSuDung)
                .ToListAsync();
        }

        public async Task<List<PhieuDieuChuyenKho>> GetAllAsync()
        {
            return await _context.PhieuDieuChuyenKhos
                .Include(p => p.ChiNhanhGui)
                .Include(p => p.ChiNhanhNhan)
                .Include(p => p.NhanVienTao)
                .Include(p => p.NhanVienXacNhan)
                .Include(p => p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct => ct.LoHang)
                        .ThenInclude(lh => lh.SanPham)
                .OrderByDescending(p => p.NgayTao)
                .ToListAsync();
        }

        public async Task<PhieuDieuChuyenKho?> GetByIdAsync(int id)
        {
            return await _context.PhieuDieuChuyenKhos
                .Include(p => p.ChiNhanhGui)
                .Include(p => p.ChiNhanhNhan)
                .Include(p => p.NhanVienTao)
                .Include(p => p.NhanVienXacNhan)
                .Include(p => p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct => ct.LoHang)
                        .ThenInclude(lh => lh.SanPham)
                .FirstOrDefaultAsync(p => p.PhieuDieuChuyenId == id);
        }

        public async Task<PhieuDieuChuyenKho> TaoYeuCauAsync(PhieuDieuChuyenKho phieu, List<ChiTietPhieuDieuChuyen> chiTiets)
        {
            _context.PhieuDieuChuyenKhos.Add(phieu);
            await _context.SaveChangesAsync();

            foreach (var ct in chiTiets)
            {
                ct.PhieuDieuChuyenId = phieu.PhieuDieuChuyenId;
                _context.ChiTietPhieuDieuChuyens.Add(ct);
            }
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(phieu.PhieuDieuChuyenId))!;
        }

        public async Task<bool> DuyetAsync(int phieuId)
        {
            var phieu = await _context.PhieuDieuChuyenKhos.FindAsync(phieuId);
            if (phieu == null || phieu.TrangThai != "ChoDuyet") return false;

            phieu.TrangThai = "DaDuyet";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TuChoiAsync(int phieuId, string lyDo)
        {
            var phieu = await _context.PhieuDieuChuyenKhos.FindAsync(phieuId);
            if (phieu == null || phieu.TrangThai != "ChoDuyet") return false;

            phieu.TrangThai = "TuChoi";
            phieu.GhiChu = $"{phieu.GhiChu} | Lý do từ chối: {lyDo}".Trim(' ', '|');
            await _context.SaveChangesAsync();
            return true;
        }

        // Nhân viên chi nhánh NGUỒN thực hiện xuất kho — trừ kho thật tại đây
        public async Task<bool> XacNhanXuatKhoAsync(int phieuId, int nhanVienId)
        {
            var phieu = await _context.PhieuDieuChuyenKhos
                .Include(p => p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct => ct.LoHang)
                .FirstOrDefaultAsync(p => p.PhieuDieuChuyenId == phieuId);

            if (phieu == null || phieu.TrangThai != "DaDuyet") return false;

            foreach (var chiTiet in phieu.ChiTietPhieuDieuChuyens)
            {
                var loHang = chiTiet.LoHang;

                // Kiểm tra lại 1 lần nữa phòng trường hợp tồn kho đã thay đổi kể từ lúc tạo yêu cầu
                if (loHang.SoLuongConLai < chiTiet.SoLuong)
                    throw new InvalidOperationException($"Lô hàng '{loHang.SoLo}' không còn đủ số lượng để xuất kho.");

                loHang.SoLuongConLai -= chiTiet.SoLuong;
                if (loHang.SoLuongConLai <= 0)
                    loHang.TrangThai = "HetHang";
            }

            phieu.TrangThai = "DangVanChuyen";
            await _context.SaveChangesAsync();

            var cacSanPham = phieu.ChiTietPhieuDieuChuyens.Select(ct => ct.LoHang.SanPhamId).Distinct();
            foreach (var sanPhamId in cacSanPham)
                await DongBoTonKhoSanPhamAsync(sanPhamId);

            return true;
        }

        // Nhân viên chi nhánh ĐÍCH xác nhận đã nhận — cộng kho tại chi nhánh đích
        public async Task<bool> XacNhanNhanHangAsync(int phieuId, int nhanVienId)
        {
            var phieu = await _context.PhieuDieuChuyenKhos
                .Include(p => p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct => ct.LoHang)
                .FirstOrDefaultAsync(p => p.PhieuDieuChuyenId == phieuId);

            if (phieu == null || phieu.TrangThai != "DangVanChuyen") return false;

            foreach (var chiTiet in phieu.ChiTietPhieuDieuChuyens)
            {
                var loHangGui = chiTiet.LoHang;

                var loHangNhan = await _context.LoHangs.FirstOrDefaultAsync(lh =>
                    lh.SanPhamId == loHangGui.SanPhamId &&
                    lh.ChiNhanhId == phieu.ChiNhanhNhanId &&
                    lh.SoLo == loHangGui.SoLo);

                if (loHangNhan != null)
                {
                    loHangNhan.SoLuongNhap += chiTiet.SoLuong;
                    loHangNhan.SoLuongConLai += chiTiet.SoLuong;
                    loHangNhan.TrangThai = "ConHang";
                }
                else
                {
                    _context.LoHangs.Add(new LoHang
                    {
                        SanPhamId = loHangGui.SanPhamId,
                        ChiNhanhId = phieu.ChiNhanhNhanId,
                        SoLo = loHangGui.SoLo,
                        NgayNhap = DateOnly.FromDateTime(DateTime.Now),
                        HanSuDung = loHangGui.HanSuDung,
                        SoLuongNhap = chiTiet.SoLuong,
                        SoLuongConLai = chiTiet.SoLuong,
                        TrangThai = "ConHang"
                    });
                }
            }

            phieu.TrangThai = "HoanTat";
            phieu.NhanVienXacNhanId = nhanVienId;
            phieu.NgayXacNhan = DateTime.Now;
            await _context.SaveChangesAsync();

            var cacSanPham = phieu.ChiTietPhieuDieuChuyens.Select(ct => ct.LoHang.SanPhamId).Distinct();
            foreach (var sanPhamId in cacSanPham)
                await DongBoTonKhoSanPhamAsync(sanPhamId);

            return true;
        }

        public async Task DongBoTonKhoSanPhamAsync(int sanPhamId)
        {
            var sanPham = await _context.SanPhams.FindAsync(sanPhamId);
            if (sanPham == null) return;

            var loConHang = await _context.LoHangs
                .Where(lh => lh.SanPhamId == sanPhamId && lh.TrangThai == "ConHang")
                .ToListAsync();

            sanPham.SoLuongTon = loConHang.Sum(lh => lh.SoLuongConLai);
            sanPham.HanSuDung = loConHang.Any() ? loConHang.Min(lh => lh.HanSuDung) : null;

            await _context.SaveChangesAsync();
        }
    }
}