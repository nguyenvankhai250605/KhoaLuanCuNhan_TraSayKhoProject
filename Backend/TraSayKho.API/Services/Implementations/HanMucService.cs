using TraSayKho.API.DTOs;
using TraSayKho.API.Models;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Services.Interfaces;

namespace TraSayKho.API.Services.Implementations
{
    public class HanMucService : IHanMucService
    {
        private readonly IHanMucRepository _repository;
        public HanMucService(IHanMucRepository repository) => _repository = repository;

        public async Task<List<HanMucDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage, HanMucDto? Result)> CreateAsync(HanMucCreateDto dto)
        {
            if (!await _repository.SanPhamExistsAsync(dto.SanPhamId))
                return (false, "Sản phẩm không tồn tại.", null);

            if (dto.ChiNhanhId.HasValue && !await _repository.ChiNhanhExistsAsync(dto.ChiNhanhId.Value))
                return (false, "Chi nhánh không tồn tại.", null);

            if (await _repository.DaTonTaiAsync(dto.SanPhamId, dto.ChiNhanhId))
                return (false, "Đã tồn tại hạn mức cho sản phẩm và chi nhánh (hoặc phạm vi chung) này. Vui lòng sửa hạn mức hiện có thay vì tạo mới.", null);

            if (dto.TonKhoToiThieu.HasValue && dto.TonKhoToiDa.HasValue && dto.TonKhoToiThieu > dto.TonKhoToiDa)
                return (false, "Tồn kho tối thiểu không được lớn hơn tồn kho tối đa.", null);

            var hanMuc = new HanMucSanPham
            {
                SanPhamId = dto.SanPhamId,
                ChiNhanhId = dto.ChiNhanhId,
                HanMucBanNgay = dto.HanMucBanNgay,
                HanMucBanTuan = dto.HanMucBanTuan,
                TonKhoToiThieu = dto.TonKhoToiThieu,
                TonKhoToiDa = dto.TonKhoToiDa,
                SoNgayLuuKhoToiDa = dto.SoNgayLuuKhoToiDa,
                TrangThai = true
            };

            var created = await _repository.AddAsync(hanMuc);
            var full = await _repository.GetByIdAsync(created.HanMucId);
            return (true, null, MapToDto(full!));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, HanMucUpdateDto dto)
        {
            if (dto.TonKhoToiThieu.HasValue && dto.TonKhoToiDa.HasValue && dto.TonKhoToiThieu > dto.TonKhoToiDa)
                return (false, "Tồn kho tối thiểu không được lớn hơn tồn kho tối đa.");

            var hanMuc = new HanMucSanPham
            {
                HanMucId = id,
                HanMucBanNgay = dto.HanMucBanNgay,
                HanMucBanTuan = dto.HanMucBanTuan,
                TonKhoToiThieu = dto.TonKhoToiThieu,
                TonKhoToiDa = dto.TonKhoToiDa,
                SoNgayLuuKhoToiDa = dto.SoNgayLuuKhoToiDa,
                TrangThai = dto.TrangThai
            };

            var success = await _repository.UpdateAsync(id, hanMuc);
            return success ? (true, null) : (false, "Không tìm thấy hạn mức.");
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        public async Task<List<CanhBaoTonKhoDto>> GetCanhBaoTonKhoAsync()
        {
            var hanMucs = await _repository.GetAllAsync();
            var result = new List<CanhBaoTonKhoDto>();

            foreach (var hm in hanMucs.Where(h => h.TrangThai))
            {
                var tonHienTai = hm.SanPham.SoLuongTon;

                if (hm.TonKhoToiThieu.HasValue && tonHienTai < hm.TonKhoToiThieu.Value)
                {
                    result.Add(new CanhBaoTonKhoDto
                    {
                        SanPhamId = hm.SanPhamId,
                        TenSanPham = hm.SanPham.TenSanPham,
                        TonKhoHienTai = tonHienTai,
                        TonKhoToiThieu = hm.TonKhoToiThieu,
                        TonKhoToiDa = hm.TonKhoToiDa,
                        LoaiCanhBao = "DuoiMuc"
                    });
                }
                else if (hm.TonKhoToiDa.HasValue && tonHienTai > hm.TonKhoToiDa.Value)
                {
                    result.Add(new CanhBaoTonKhoDto
                    {
                        SanPhamId = hm.SanPhamId,
                        TenSanPham = hm.SanPham.TenSanPham,
                        TonKhoHienTai = tonHienTai,
                        TonKhoToiThieu = hm.TonKhoToiThieu,
                        TonKhoToiDa = hm.TonKhoToiDa,
                        LoaiCanhBao = "VuotMuc"
                    });
                }
            }

            return result;
        }

        // ==== HÀM QUAN TRỌNG NHẤT: dùng khi tạo đơn hàng để kiểm tra hạn mức bán ====
        public async Task<(bool HopLe, string? LyDoTuChoi)> KiemTraHanMucBanAsync(int sanPhamId, int? chiNhanhId, int soLuongDatMua)
        {
            var hanMuc = await _repository.GetApDungChoSanPhamAsync(sanPhamId, chiNhanhId);
            if (hanMuc == null) return (true, null);   // không có hạn mức nào cấu hình → không giới hạn

            var homNay = DateTime.Now.Date;

            if (hanMuc.HanMucBanNgay.HasValue)
            {
                var daBanHomNay = await _repository.DemSoLuongDaBanTrongKhoangAsync(sanPhamId, homNay, homNay.AddDays(1).AddSeconds(-1));
                if (daBanHomNay + soLuongDatMua > hanMuc.HanMucBanNgay.Value)
                    return (false, $"Sản phẩm đã đạt hạn mức bán trong ngày ({hanMuc.HanMucBanNgay} sản phẩm/ngày). Đã bán: {daBanHomNay}, còn lại có thể mua: {Math.Max(0, hanMuc.HanMucBanNgay.Value - daBanHomNay)}.");
            }

            if (hanMuc.HanMucBanTuan.HasValue)
            {
                var dauTuan = homNay.AddDays(-(int)homNay.DayOfWeek);
                var daBanTrongTuan = await _repository.DemSoLuongDaBanTrongKhoangAsync(sanPhamId, dauTuan, dauTuan.AddDays(7).AddSeconds(-1));
                if (daBanTrongTuan + soLuongDatMua > hanMuc.HanMucBanTuan.Value)
                    return (false, $"Sản phẩm đã đạt hạn mức bán trong tuần ({hanMuc.HanMucBanTuan} sản phẩm/tuần). Đã bán: {daBanTrongTuan}, còn lại có thể mua: {Math.Max(0, hanMuc.HanMucBanTuan.Value - daBanTrongTuan)}.");
            }

            return (true, null);
        }

        private static HanMucDto MapToDto(HanMucSanPham h) => new()
        {
            HanMucId = h.HanMucId,
            SanPhamId = h.SanPhamId,
            TenSanPham = h.SanPham.TenSanPham,
            ChiNhanhId = h.ChiNhanhId,
            TenChiNhanh = h.ChiNhanh?.TenChiNhanh,
            HanMucBanNgay = h.HanMucBanNgay,
            HanMucBanTuan = h.HanMucBanTuan,
            TonKhoToiThieu = h.TonKhoToiThieu,
            TonKhoToiDa = h.TonKhoToiDa,
            SoNgayLuuKhoToiDa = h.SoNgayLuuKhoToiDa,
            TrangThai = h.TrangThai
        };
    }
}