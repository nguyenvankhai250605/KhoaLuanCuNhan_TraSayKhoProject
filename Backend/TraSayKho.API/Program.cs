using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Repositories.Implementations;
using TraSayKho.API.Services.Interfaces;
using TraSayKho.API.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TraSayKhoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TraSayKhoDB")));

// ==== ĐĂNG KÝ REPOSITORY ====
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IKhuyenMaiRepository, KhuyenMaiRepository>();
builder.Services.AddScoped<IDanhGiaRepository, DanhGiaRepository>();
builder.Services.AddScoped<IThongKeRepository, ThongKeRepository>();
builder.Services.AddScoped<IHinhAnhSanPhamRepository, HinhAnhSanPhamRepository>();
builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
builder.Services.AddScoped<IChatbotRepository, ChatbotRepository>();
builder.Services.AddScoped<IChiNhanhRepository, ChiNhanhRepository>();
builder.Services.AddScoped<ILoHangRepository, LoHangRepository>();
builder.Services.AddScoped<IPhieuDieuChuyenRepository, PhieuDieuChuyenRepository>();

// ==== ĐĂNG KÝ SERVICE ====
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>();
builder.Services.AddScoped<IDanhGiaService, DanhGiaService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IHinhAnhSanPhamService, HinhAnhSanPhamService>();
builder.Services.AddScoped<IThongBaoService, ThongBaoService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddScoped<IChiNhanhService, ChiNhanhService>();
builder.Services.AddScoped<ILoHangService, LoHangService>();
builder.Services.AddScoped<IPhieuDieuChuyenService, PhieuDieuChuyenService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddHttpClient();
// builder.Services.AddOpenApi();
// ==== THAY dòng AddOpenApi() bằng 2 dòng này ====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    // ==== THAY dòng MapOpenApi() bằng 2 dòng này ====
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
