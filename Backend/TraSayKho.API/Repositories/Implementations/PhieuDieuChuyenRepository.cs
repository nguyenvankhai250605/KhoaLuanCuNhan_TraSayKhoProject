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

        public async Task<LoHang?> GetLoHangByIdAsync(int loHangId)
        {
            return await _context.LoHangs.FirstOrDefaultAsync(lh => lh.LoHangId == loHangId);
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

        public async Task<PhieuDieuChuyenKho> CreateAsync(PhieuDieuChuyenKho phieu, List<ChiTietPhieuDieuChuyen> chiTiets)
        {
            _context.PhieuDieuChuyenKhos.Add(phieu);
            await _context.SaveChangesAsync();   // lưu trước để có PhieuDieuChuyenId

            foreach (var ct in chiTiets)
            {
                ct.PhieuDieuChuyenId = phieu.PhieuDieuChuyenId;
                _context.ChiTietPhieuDieuChuyens.Add(ct);
            }
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(phieu.PhieuDieuChuyenId))!;
        }

        public async Task<bool> XacNhanAsync(int phieuId, int nhanVienXacNhanId)
        {
            var phieu = await _context.PhieuDieuChuyenKhos
                .Include(p => p.ChiTietPhieuDieuChuyens)
                    .ThenInclude(ct => ct.LoHang)
                .FirstOrDefaultAsync(p => p.PhieuDieuChuyenId == phieuId);

            if (phieu == null || phieu.TrangThai != "ChoXacNhan") return false;

            foreach (var chiTiet in phieu.ChiTietPhieuDieuChuyens)
            {
                var loHangGui = chiTiet.LoHang;

                // 1. Trừ ở lô gốc (chi nhánh gửi)
                loHangGui.SoLuongConLai -= chiTiet.SoLuong;
                if (loHangGui.SoLuongConLai <= 0)
                    loHangGui.TrangThai = "HetHang";

                // 2. Tìm hoặc tạo lô tương ứng ở chi nhánh nhận (giữ nguyên Số lô + Hạn sử dụng)
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

            phieu.TrangThai = "DaXacNhan";
            phieu.NhanVienXacNhanId = nhanVienXacNhanId;
            phieu.NgayXacNhan = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HuyAsync(int phieuId)
        {
            var phieu = await _context.PhieuDieuChuyenKhos.FindAsync(phieuId);
            if (phieu == null || phieu.TrangThai != "ChoXacNhan") return false;

            phieu.TrangThai = "DaHuy";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}