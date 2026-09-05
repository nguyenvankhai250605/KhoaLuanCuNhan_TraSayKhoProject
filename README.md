# 🍵 Ứng dụng bán trà sấy khô tích hợp Chatbot AI (Mô hình đa chi nhánh)

Web quản trị bán trà sấy khô, xây dựng bằng **ASP.NET Core Web API**, **Entity Framework Core**, **SQL Server**, theo kiến trúc **N-Layer**. Hỗ trợ **đa chi nhánh** (tồn kho theo lô/FEFO, xả kho đa bậc tự động, điều chuyển kho), **Chatbot AI** tư vấn sản phẩm, và bảo mật **JWT + phân quyền theo vai trò lẫn chi nhánh**.

## 🛠️ Yêu cầu hệ thống

- **.NET SDK**: 8.0 trở lên
- **Cơ sở dữ liệu**: Microsoft SQL Server (2019+), kèm SSMS
- **IDE khuyên dùng**: Visual Studio Code (kèm C# Dev Kit)

## 🚀 Hướng dẫn cài đặt và chạy ứng dụng

### Bước 1: Cơ sở dữ liệu

Mở SSMS → **New Query** → dán toàn bộ `Database/Database_TraSayKho.sql` → **Execute (F5)**.

> ✅ Script đã gồm sẵn: 2 chi nhánh mẫu, 3 bậc giảm giá xả kho, danh mục, sản phẩm, lô hàng, tài khoản thử nghiệm, 1 đơn hàng demo.

### Bước 2: Cấu hình kết nối Database

Sửa `Backend/TraSayKho.API/appsettings.json`:

```json
"ConnectionStrings": {
  "TraSayKhoDB": "Server=TEN_SERVER_CUA_BAN\\SQLEXPRESS;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Tìm tên server bằng SSMS. Nếu có dấu `\`, gõ `\\` trong JSON.

### Bước 3: Cấu hình khóa bí mật

```bash
cd Backend/TraSayKho.API
dotnet user-secrets init
dotnet user-secrets set "GeminiApi:ApiKey" "API_KEY_GEMINI_CUA_BAN"
dotnet user-secrets set "JwtSettings:SecretKey" "day-la-chuoi-bi-mat-rat-dai-va-kho-doan-cho-du-an-tra-say-kho-2026"
```

Thêm vào `appsettings.json`:

```json
{
  "GeminiApi": { "Model": "gemini-3.6-flash" },
  "JwtSettings": {
    "Issuer": "TraSayKhoAPI",
    "Audience": "TraSayKhoUsers",
    "ExpiryMinutes": 1440
  }
}
```

### Bước 4: Khởi động

```bash
dotnet restore
dotnet run
```

Mở `http://localhost:{port}/swagger`.

### Bước 5: Dữ liệu mẫu (dự phòng)

Xóa `TraSayKhoDB` trong SSMS, chạy lại `.sql` như Bước 1.

## 🔐 Đăng nhập & Phân quyền

### Lấy Token trên Swagger

1. `POST /api/Auth/dangnhap` → copy `token`.
2. Bấm **Authorize 🔒** → dán **đúng chuỗi token** (KHÔNG gõ thêm chữ "Bearer", Swagger tự thêm) → **Authorize** → **Close**.

> ⚠️ Tài khoản mẫu cũ (`admin`, `khachhang01`...) **KHÔNG đăng nhập được** (mật khẩu giả lập). Tạo tài khoản mới qua `POST /api/Auth/dangky` (khách) hoặc `POST /api/Auth/taonhanvien` (admin/nhân viên).

### Cấp độ phân quyền

| Cấp | Điều kiện | Phạm vi truy cập |
|---|---|---|
| **Admin tổng** | `ChiNhanhId = null` trong token | Xem/sửa **mọi** chi nhánh, cấu hình Bậc giảm giá xả kho |
| **Nhân viên** | `ChiNhanhId` cụ thể | Chỉ xem/sửa dữ liệu **đúng chi nhánh mình** (Lô hàng, Đơn hàng, Phiếu điều chuyển...). Cố truy cập chi nhánh khác → `403 Forbidden` |
| **Khách hàng** | Vai trò `KhachHang` | Không truy cập được API quản trị |

### Chi nhánh mẫu

| Chi nhánh | Địa chỉ |
|---|---|
| Chi nhánh Quận 1 | 123 Nguyễn Huệ, Quận 1, TP.HCM |
| Chi nhánh Thủ Đức | 45 Võ Văn Ngân, Thủ Đức, TP.HCM |

## 🏷️ Cơ chế khuyến mãi (2 loại riêng biệt)

| | `KhuyenMai` | `Xả kho theo Lô` |
|---|---|---|
| Mục đích | Marketing chung (lễ, Tết, khai trương...) | Xử lý hàng tồn cận hạn |
| Cách dùng | Khách nhập mã code lúc thanh toán | Tự động hiển thị giá giảm trên sản phẩm |
| Phạm vi | Toàn đơn hàng | Riêng 1 lô hàng cụ thể |

**Xả kho hoạt động theo 2 tầng ưu tiên:**
1. Nếu nhân viên **bấm tay** (`PUT /api/LoHang/{id}/xakho`) và còn hiệu lực → dùng mức đó.
2. Nếu không → hệ thống **tự động** áp mức giảm theo `BacGiamGiaXaKho` (cấu hình sẵn: ≤90 ngày giảm 15%, ≤30 ngày giảm 30%, ≤7 ngày giảm 50%), tính lại mỗi lần truy vấn, không cần tiến trình nền.

## 📋 Danh sách API chính

| Module | Endpoint | Ghi chú |
|---|---|---|
| Xác thực | `POST /api/Auth/dangky`, `.../taonhanvien`, `.../dangnhap` | Không cần đăng nhập trước |
| Sản phẩm | `GET/POST/PUT/DELETE /api/SanPham` 🔒 | Tồn kho tự đồng bộ từ Lô hàng |
| Danh mục / Khuyến mãi / Đánh giá | CRUD tương ứng 🔒 | |
| Đơn hàng | `GET /api/DonHang`, `PUT .../trangthai` 🔒🏢 | Lọc theo chi nhánh với Nhân viên |
| Khách hàng | `GET/PUT /api/KhachHang`, `.../trangthai` 🔒 | |
| Chi nhánh | `GET/POST/PUT/DELETE /api/ChiNhanh` 🔒 | Tạo/sửa/xóa: chỉ Admin |
| Lô hàng | `GET/POST /api/LoHang`, `.../sanpham/{id}`, `.../saphethan` 🔒🏢 | Trả kèm `giaSauGiam`, `laGiamGiaTuDong` |
| Xả kho thủ công | `PUT /api/LoHang/{id}/xakho`, `.../huyxakho` 🔒🏢 | |
| **Bậc giảm giá xả kho** | `GET/POST/PUT/DELETE /api/BacGiamGia` 🔒 (chỉ Admin) | Cấu hình các ngưỡng giảm giá tự động |
| Phiếu điều chuyển kho | `GET/POST /api/PhieuDieuChuyen`, `.../xacnhan`, `.../huy` 🔒🏢 | Bên gửi tạo/hủy, bên nhận xác nhận |
| Thống kê | `GET /api/ThongKe/...` 🔒🏢 | Nhân viên bị ép về đúng chi nhánh mình dù truyền tham số khác |
| Hình ảnh / Thông báo | CRUD tương ứng 🔒 | |
| Chatbot AI | `POST /api/Chatbot/chat`, `.../lichsu/{id}`, `PUT .../dongphien/{id}` | Không cần đăng nhập (tạm thời) |

> 🔒 = yêu cầu đăng nhập · 🏢 = có kiểm tra quyền theo chi nhánh. Chi tiết đầy đủ tại Swagger UI.

## 📌 Khắc phục sự cố thường gặp

- **Sai thư mục khi chạy `dotnet`**: `cd Backend/TraSayKho.API` trước.
- **Sai server SQL**: kiểm tra bằng SSMS, dùng `\\` trong JSON.
- **`DbSet` không tồn tại**: đối chiếu `Data/TraSayKhoDbContext.cs`.
- **`X?` → `X` / thừa `??`**: thêm/xóa `?? giá_trị_mặc_định` tùy trường hợp.
- **Chatbot lỗi API key / lỗi 404 model**: kiểm tra `user-secrets`, đổi `GeminiApi:Model` theo gợi ý trong thông báo lỗi.
- **Đổi database (thêm bảng/cột)**: chạy lại `.sql` mới, rồi scaffold lại:
```bash
  dotnet ef dbcontext scaffold "Server=TEN_SERVER;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context TraSayKhoDbContext --context-dir Data --no-onconfiguring --force
```
- **`401 Unauthorized`**: chưa đăng nhập/Authorize, hoặc token hết hạn (24h) — đăng nhập lại. Cũng có thể do vừa restart server làm Swagger mất trạng thái Authorize đã lưu — bấm Authorize lại là được.
- **`403 Forbidden`**: đã đăng nhập đúng, nhưng cố thao tác dữ liệu **không thuộc chi nhánh mình** (chỉ Admin mới vượt được giới hạn này).
- **Nhập token bị lặp "Bearer Bearer"**: chỉ dán đúng token vào ô Value, không tự gõ thêm chữ "Bearer".