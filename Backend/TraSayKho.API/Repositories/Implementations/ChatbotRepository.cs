using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;

namespace TraSayKho.API.Repositories.Implementations
{
    public class ChatbotRepository : IChatbotRepository
    {
        private readonly TraSayKhoDbContext _context;
        public ChatbotRepository(TraSayKhoDbContext context) => _context = context;

        public async Task<bool> KhachHangExistsAsync(int khachHangId)
        {
            return await _context.KhachHangs.AnyAsync(kh => kh.KhachHangId == khachHangId);
        }

        public async Task<CuocHoiThoai?> GetCuocHoiThoaiDangMoAsync(int khachHangId)
        {
            return await _context.CuocHoiThoais
                .Where(cht => cht.KhachHangId == khachHangId && cht.TrangThai == "DangMo")
                .OrderByDescending(cht => cht.NgayBatDau)
                .FirstOrDefaultAsync();
        }

        public async Task<CuocHoiThoai> TaoCuocHoiThoaiMoiAsync(int khachHangId)
        {
            var cuocHoiThoai = new CuocHoiThoai
            {
                KhachHangId = khachHangId,
                NgayBatDau = DateTime.Now,
                TrangThai = "DangMo"
            };

            _context.CuocHoiThoais.Add(cuocHoiThoai);
            await _context.SaveChangesAsync();
            return cuocHoiThoai;
        }

        public async Task<TinNhan?> GetTinNhanGanNhatAsync(int cuocHoiThoaiId)
        {
            return await _context.TinNhans
                .Where(tn => tn.CuocHoiThoaiId == cuocHoiThoaiId)
                .OrderByDescending(tn => tn.ThoiGianGui)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DongPhienAsync(int cuocHoiThoaiId)
        {
            var cuocHoiThoai = await _context.CuocHoiThoais.FindAsync(cuocHoiThoaiId);
            if (cuocHoiThoai == null) return false;

            cuocHoiThoai.TrangThai = "DaDong";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CuocHoiThoai?> GetCuocHoiThoaiByIdAsync(int cuocHoiThoaiId)
        {
            return await _context.CuocHoiThoais.FindAsync(cuocHoiThoaiId);
        }

        public async Task<TinNhan> AddTinNhanAsync(int cuocHoiThoaiId, string nguoiGui, string noiDung)
        {
            var tinNhan = new TinNhan
            {
                CuocHoiThoaiId = cuocHoiThoaiId,
                NguoiGui = nguoiGui,
                NoiDung = noiDung,
                ThoiGianGui = DateTime.Now
            };

            _context.TinNhans.Add(tinNhan);
            await _context.SaveChangesAsync();
            return tinNhan;
        }

        public async Task<List<TinNhan>> GetLichSuGanNhatAsync(int cuocHoiThoaiId, int soLuong)
        {
            var tinNhans = await _context.TinNhans
                .Where(tn => tn.CuocHoiThoaiId == cuocHoiThoaiId)
                .OrderByDescending(tn => tn.ThoiGianGui)
                .Take(soLuong)
                .ToListAsync();

            tinNhans.Reverse();
            return tinNhans;
        }

        public async Task<List<TinNhan>> GetToanBoLichSuAsync(int cuocHoiThoaiId)
        {
            return await _context.TinNhans
                .Where(tn => tn.CuocHoiThoaiId == cuocHoiThoaiId)
                .OrderBy(tn => tn.ThoiGianGui)
                .ToListAsync();
        }

        public async Task<List<SanPham>> GetSanPhamDangBanKemChiTietAsync()
        {
            return await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.CongDungs)
                .Include(sp => sp.ThanhPhans)
                .Where(sp => sp.TrangThai == "DangBan")
                .ToListAsync();
        }
    }
}