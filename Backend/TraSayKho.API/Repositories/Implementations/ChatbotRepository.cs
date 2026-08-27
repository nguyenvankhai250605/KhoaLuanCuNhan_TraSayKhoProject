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

        public async Task<CuocHoiThoai> GetOrCreateCuocHoiThoaiAsync(int khachHangId)
        {
            // Tìm cuộc hội thoại đang mở gần nhất của khách hàng này
            var cuocHoiThoai = await _context.CuocHoiThoais
                .Where(cht => cht.KhachHangId == khachHangId && cht.TrangThai == "DangMo")
                .OrderByDescending(cht => cht.NgayBatDau)
                .FirstOrDefaultAsync();

            if (cuocHoiThoai != null)
                return cuocHoiThoai;

            // Chưa có thì tạo mới
            cuocHoiThoai = new CuocHoiThoai
            {
                KhachHangId = khachHangId,
                NgayBatDau = DateTime.Now,
                TrangThai = "DangMo"
            };

            _context.CuocHoiThoais.Add(cuocHoiThoai);
            await _context.SaveChangesAsync();
            return cuocHoiThoai;
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

            tinNhans.Reverse(); // đảo lại đúng thứ tự thời gian (cũ → mới)
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