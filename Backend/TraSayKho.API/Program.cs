using Microsoft.EntityFrameworkCore;
using TraSayKho.API.Data;
using TraSayKho.API.Repositories.Interfaces;
using TraSayKho.API.Repositories.Implementations;
using TraSayKho.API.Services.Interfaces;
using TraSayKho.API.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

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
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

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
builder.Services.AddScoped<IAuthService, AuthService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddHttpClient();
// builder.Services.AddOpenApi();
// ==== THAY dòng AddOpenApi() bằng 2 dòng này ====
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Dán token vào đây, dạng: Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// ==== CẤU HÌNH JWT AUTHENTICATION ====
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

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

app.UseAuthentication();   // ← DÒNG NÀY PHẢI ĐỨNG TRƯỚC UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
