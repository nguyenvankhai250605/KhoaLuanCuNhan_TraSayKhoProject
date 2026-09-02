# 🍵 Ứng dụng bán trà sấy khô tích hợp Chatbot AI (Mô hình đa chi nhánh)

Đây là dự án Web quản trị bán trà sấy khô được xây dựng bằng **ASP.NET Core Web API**, **Entity Framework Core**, và **SQL Server**, theo kiến trúc **N-Layer** (Controller → Service → Repository). Hệ thống hỗ trợ quản lý **nhiều chi nhánh**, theo dõi tồn kho theo **lô hàng** (hạn sử dụng, FEFO), tích hợp **Chatbot AI** tư vấn sản phẩm, và bảo mật bằng **JWT Authentication** (đăng nhập, phân quyền theo vai trò).

## 🛠️ Yêu cầu hệ thống

- **.NET SDK**: 8.0 trở lên
- **Cơ sở dữ liệu**: Microsoft SQL Server (2019 hoặc mới hơn), kèm SSMS
- **IDE khuyên dùng**: Visual Studio Code (kèm extension C# Dev Kit)

## 🚀 Hướng dẫn cài đặt và chạy ứng dụng

### Bước 1: Chuẩn bị Cơ sở dữ liệu (Database)

1. Mở **SQL Server Management Studio (SSMS)**.
2. Đăng nhập vào SQL Server (Windows Authentication).
3. Mở **New Query**, copy toàn bộ nội dung file `Database/Database_TraSayKho.sql`, dán vào và **Execute (F5)** để tạo database, các bảng và dữ liệu mẫu.

> ✅ **Không cần chạy thêm lệnh nào khác** — script này đã bao gồm sẵn 2 chi nhánh mẫu, danh mục, sản phẩm, lô hàng, tài khoản thử nghiệm và 1 đơn hàng demo.

### Bước 2: Cấu hình kết nối Database

Mở file `Backend/TraSayKho.API/appsettings.json` và sửa lại thông tin cấu hình SQL Server cho phù hợp với máy của bạn.

> ⚠️ **Lưu ý:** Mỗi máy có tên SQL Server khác nhau. Tìm tên server bằng cách mở SSMS, xem ở ô "Server name" lúc đăng nhập.

```json
"ConnectionStrings": {
  "TraSayKhoDB": "Server=TEN_SERVER_CUA_BAN\\SQLEXPRESS;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Nếu tên server có dấu `\`, trong file JSON phải gõ **2 dấu `\\`** liền nhau.

### Bước 3: Cấu hình các khóa bí mật (Secrets)

Mở Terminal tại `Backend/TraSayKho.API`, chạy:

```bash
dotnet user-secrets init
dotnet user-secrets set "GeminiApi:ApiKey" "API_KEY_GEMINI_CUA_BAN"
dotnet user-secrets set "JwtSettings:SecretKey" "day-la-chuoi-bi-mat-rat-dai-va-kho-doan-cho-du-an-tra-say-kho-2026"
```

> - **Gemini API key**: lấy miễn phí tại https://aistudio.google.com/apikey. Có thể dùng chung 1 key cho cả nhóm — liên hệ Khải nếu chưa có.
> - **JWT SecretKey**: chuỗi bí mật dùng để ký/xác minh token đăng nhập, nên dài (≥32 ký tự), có thể tự nghĩ ra hoặc dùng đúng ví dụ trên.
> - ⚠️ **Không đặt các giá trị này trực tiếp trong `appsettings.json`** — lệnh `user-secrets` lưu ở nơi an toàn ngoài project, tránh lộ khi push Git.

Mở `appsettings.json`, thêm phần cấu hình không nhạy cảm:

```json
{
  "ConnectionStrings": {
    "TraSayKhoDB": "..."
  },
  "GeminiApi": {
    "Model": "gemini-3.6-flash"
  },
  "JwtSettings": {
    "Issuer": "TraSayKhoAPI",
    "Audience": "TraSayKhoUsers",
    "ExpiryMinutes": 1440
  }
}
```

### Bước 4: Khởi động ứng dụng

```bash
dotnet restore
dotnet run
```

Mở trình duyệt vào `http://localhost:{port}/swagger`.

### Bước 5: Chạy lại dữ liệu mẫu (Không bắt buộc — Chỉ dùng làm dự phòng)

Nếu muốn đặt lại dữ liệu gốc: xóa database `TraSayKhoDB` cũ trong SSMS, chạy lại file `.sql` như Bước 1.

## 🔐 Đăng nhập & Sử dụng API có bảo mật

Từ đợt cập nhật này, **hầu hết API quản trị** (Sản phẩm, Đơn hàng, Chi nhánh, Lô hàng...) **yêu cầu đăng nhập** mới gọi được. Riêng `Auth` (đăng ký/đăng nhập) và `Chatbot` vẫn mở tự do.

### Cách lấy Token và test trên Swagger

1. Gọi `POST /api/Auth/dangnhap` với tài khoản hợp lệ → copy giá trị `token` trong response (chuỗi dài bắt đầu `eyJ...`).
2. Bấm nút **Authorize 🔒** ở đầu trang Swagger.
3. Trong ô Value, **chỉ dán đúng chuỗi token** — **KHÔNG gõ thêm chữ "Bearer"** (Swagger tự thêm sẵn rồi, gõ thêm sẽ bị lặp "Bearer Bearer" gây lỗi 401).
4. Bấm **Authorize** → thấy chữ "Authorized" → bấm **Close**.
5. Từ giờ mọi request đều tự động kèm token.

> ⚠️ **Tài khoản mẫu cũ (`admin`, `khachhang01`...) KHÔNG đăng nhập được** — mật khẩu của chúng là chuỗi giả lập, chưa qua mã hóa BCrypt thật. Cần **tạo tài khoản mới** qua `POST /api/Auth/dangky` (khách hàng) hoặc `POST /api/Auth/taonhanvien` (admin/nhân viên) rồi đăng nhập lại bằng tài khoản đó.

### Chi nhánh mẫu

| Chi nhánh | Địa chỉ | Ghi chú |
|---|---|---|
| Chi nhánh Quận 1 | 123 Nguyễn Huệ, Quận 1, TP.HCM | Chi nhánh chính (trụ sở) |
| Chi nhánh Thủ Đức | 45 Võ Văn Ngân, Thủ Đức, TP.HCM | Chi nhánh phụ |

## 📋 Danh sách API chính

| Module | Endpoint | Ghi chú |
|---|---|---|
| **Xác thực** | `POST /api/Auth/dangky`, `.../taonhanvien`, `.../dangnhap` | Đăng ký khách hàng, tạo tài khoản nhân viên, đăng nhập nhận JWT token. **Không yêu cầu đăng nhập trước.** |
| Sản phẩm | `GET/POST/PUT/DELETE /api/SanPham` 🔒 | Đầy đủ CRUD, xóa mềm. `SoLuongTon`/`HanSuDung` tự đồng bộ từ Lô hàng |
| Danh mục | `GET/POST/PUT/DELETE /api/DanhMuc` 🔒 | Đầy đủ CRUD, xóa mềm |
| Khuyến mãi | `GET/POST/PUT/DELETE /api/KhuyenMai` 🔒 | Đầy đủ CRUD, xóa mềm |
| Đơn hàng | `GET /api/DonHang`, `PUT .../trangthai` 🔒 | Chỉ cập nhật trạng thái theo đúng thứ tự |
| Khách hàng | `GET/PUT /api/KhachHang`, `PUT .../trangthai` 🔒 | Không tạo mới (dùng `Auth/dangky`) |
| Đánh giá | `GET/DELETE /api/DanhGia` 🔒 | Chỉ đọc và xóa (kiểm duyệt) |
| Chi nhánh | `GET/POST/PUT/DELETE /api/ChiNhanh` 🔒 | Quản lý danh sách chi nhánh |
| Lô hàng | `GET/POST /api/LoHang`, `.../sanpham/{id}`, `.../saphethan` 🔒 | Nhập lô, tra cứu, cảnh báo cận hạn |
| Xả kho | `PUT /api/LoHang/{id}/xakho`, `.../huyxakho` 🔒 | Gán/hủy giảm giá riêng theo lô |
| Phiếu điều chuyển kho | `GET/POST /api/PhieuDieuChuyen`, `.../xacnhan`, `.../huy` 🔒 | Chuyển hàng giữa 2 chi nhánh |
| Thống kê | `GET /api/ThongKe/...` (có `?chiNhanhId=`) 🔒 | Bỏ trống = toàn hệ thống |
| Hình ảnh sản phẩm | `GET/POST/DELETE /api/SanPham/{id}/HinhAnhSanPham` 🔒 | Upload/xóa ảnh |
| Thông báo | `GET/POST /api/ThongBao` 🔒 | Gửi cho 1 hoặc toàn bộ khách hàng |
| Chatbot AI | `POST /api/Chatbot/chat`, `.../lichsu/{id}`, `PUT .../dongphien/{id}` | Tư vấn bằng Gemini, tự đóng phiên sau 60 phút. **Không yêu cầu đăng nhập** (tạm thời, chờ luồng Mobile hoàn chỉnh) |

> 🔒 = yêu cầu đăng nhập (Bearer token). Xem chi tiết đầy đủ tại Swagger UI.

## 📌 Khắc phục sự cố thường gặp

- **Lỗi `Could not find any project in ...`**: Sai thư mục. Chạy `cd Backend/TraSayKho.API` trước.
- **Lỗi `The server was not found or was not accessible`**: Sai tên server hoặc dùng nhầm `/` thay `\`.
- **Lỗi `does not contain a definition for 'XxxYyy'`**: Sai tên `DbSet`. Đối chiếu `Data/TraSayKhoDbContext.cs`.
- **Lỗi `Cannot implicitly convert type 'X?' to 'X'`**: Thiếu `?? giá_trị_mặc_định`.
- **Lỗi `Operator '??' cannot be applied...`**: Thừa `?? giá_trị_mặc_định`, xóa đi.
- **Chatbot lỗi "thiếu API key"**: Chưa cấu hình `user-secrets` (Bước 3).
- **Chatbot lỗi mã 404**: Model Gemini bị ngừng hỗ trợ, đọc thông báo lỗi để lấy tên model mới, cập nhật `GeminiApi:Model`.
- **Sau khi pull code có thay đổi database**: chạy lại `.sql` mới nhất, rồi scaffold lại Model:
```bash
  dotnet ef dbcontext scaffold "Server=TEN_SERVER;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context TraSayKhoDbContext --context-dir Data --no-onconfiguring --force
```
- **Gọi API bị `401 Unauthorized`**: Chưa đăng nhập/chưa Authorize trên Swagger, hoặc token đã hết hạn (24h) — đăng nhập lại lấy token mới.
- **Vẫn `401` dù đã nhập token**: Kiểm tra ô Value trong Swagger Authorize — chỉ dán đúng token, không tự gõ thêm chữ "Bearer" phía trước (Swagger tự thêm sẵn).
- **Máy mỗi người có cấu hình SQL Server khác nhau**: luôn tự sửa `appsettings.json` theo máy mình.