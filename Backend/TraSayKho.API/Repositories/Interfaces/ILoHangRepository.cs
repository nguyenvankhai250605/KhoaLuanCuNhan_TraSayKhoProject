using TraSayKho.API.Models;

namespace TraSayKho.API.Repositories.Interfaces
{
    public interface ILoHangRepository
    {
        Task<bool> SanPhamExistsAsync(int sanPhamId);
        Task<bool> ChiNhanhExistsAsync(int chiNhanhId);
        Task<List<LoHang>> GetAllAsync();
        Task<LoHang?> GetByIdAsync(int id);
        Task<List<LoHang>> GetBySanPhamAsync(int sanPhamId);
        Task<List<LoHang>> GetSapHetHanAsync(int soNgayNguong);
        Task<LoHang> AddAsync(LoHang loHang);
        Task<bool> UpdateXaKhoAsync(int loHangId, decimal? mucGiam, DateOnly? tuNgay, DateOnly? denNgay);
        Task DongBoTonKhoSanPhamAsync(int sanPhamId);
    }
}