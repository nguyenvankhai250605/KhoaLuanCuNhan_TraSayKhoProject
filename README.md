# 🍵 Ứng dụng bán trà sấy khô tích hợp Chatbot AI

Đây là dự án Web quản trị bán trà sấy khô được xây dựng bằng **ASP.NET Core Web API**, **Entity Framework Core**, và **SQL Server**, theo kiến trúc **N-Layer** (Controller → Service → Repository).

## 🛠️ Yêu cầu hệ thống

- **.NET SDK**: 8.0 trở lên
- **Cơ sở dữ liệu**: Microsoft SQL Server (2019 hoặc mới hơn), kèm SSMS
- **IDE khuyên dùng**: Visual Studio Code (kèm extension C# Dev Kit)

## 🚀 Hướng dẫn cài đặt và chạy ứng dụng

### Bước 1: Chuẩn bị Cơ sở dữ liệu (Database)

1. Mở **SQL Server Management Studio (SSMS)**.
2. Đăng nhập vào SQL Server (Windows Authentication).
3. Mở **New Query**, copy toàn bộ nội dung file `Database/Database_TraSayKho.sql`, dán vào và **Execute (F5)** để tạo database, các bảng và dữ liệu mẫu.

> ✅ **Không cần chạy thêm lệnh nào khác** — script này đã bao gồm sẵn toàn bộ danh mục, sản phẩm mẫu, tài khoản thử nghiệm và 1 đơn hàng demo.

### Bước 2: Cấu hình kết nối Database

Mở file `Backend/TraSayKho.API/appsettings.json` và sửa lại thông tin cấu hình SQL Server cho phù hợp với máy của bạn.

> ⚠️ **Lưu ý:** Mỗi máy có tên SQL Server khác nhau (ví dụ `KHAINGUYEN\SQLEXPRESS`, `TENMAY-PC\SQLEXPRESS`...). Bạn **CẦN PHẢI** sửa lại phần này — tìm tên server của mình bằng cách mở SSMS, xem ở ô "Server name" lúc đăng nhập.

Ví dụ cấu hình dùng Windows Authentication:

```json
"ConnectionStrings": {
  "TraSayKhoDB": "Server=TEN_SERVER_CUA_BAN\\SQLEXPRESS;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Nếu tên server có dấu `\`, trong file JSON phải gõ **2 dấu `\\`** liền nhau (do quy tắc escape của JSON).

### Bước 3: Cấu hình API Key cho Chatbot AI

Chatbot sử dụng Gemini API để tư vấn sản phẩm. Mở Terminal tại `Backend/TraSayKho.API`, chạy:

```bash
dotnet user-secrets init
dotnet user-secrets set "GeminiApi:ApiKey" "API_KEY_CUA_BAN"
```

> Lấy API key miễn phí tại: https://aistudio.google.com/apikey (đăng nhập bằng tài khoản Google, không cần thẻ thanh toán). Có thể dùng chung 1 key cho cả nhóm — liên hệ Khải để lấy key nếu chưa có.

> ⚠️ **Không đặt API key trực tiếp trong `appsettings.json`** — lệnh `user-secrets` sẽ lưu key ở nơi an toàn ngoài project, tránh bị lộ khi push Git.

> Nếu bỏ qua bước này, toàn bộ hệ thống vẫn chạy bình thường — chỉ riêng tính năng Chatbot sẽ báo lỗi "thiếu API key" khi gọi tới.

### Bước 4: Khởi động ứng dụng

Mở Terminal tại thư mục `Backend/TraSayKho.API`, chạy lần lượt:

```bash
dotnet restore
dotnet run
```

> 🌟 **Điểm cộng đặc biệt:** Toàn bộ dữ liệu mẫu (danh mục, sản phẩm, tài khoản, đơn hàng demo) đã được nạp sẵn ngay từ Bước 1 thông qua script SQL. Bạn **không cần chạy thêm bước nạp dữ liệu riêng nào**.

### Bước 5: Chạy lại dữ liệu mẫu (Không bắt buộc — Chỉ dùng làm dự phòng)

Nếu vì lý do nào đó bạn muốn đặt lại dữ liệu gốc từ đầu:

1. Xóa database `TraSayKhoDB` cũ trong SSMS (chuột phải → Delete).
2. Chạy lại toàn bộ file `Database/Database_TraSayKho.sql` như Bước 1.

## 🌐 Trải nghiệm ứng dụng

Sau khi khởi động thành công, mở trình duyệt và truy cập:

- **Swagger API (test toàn bộ endpoint):** `http://localhost:{port}/swagger`
  *(port hiển thị ngay trong Terminal, dòng `Now listening on...`)*

### Tài khoản đăng nhập mẫu

| Vai trò | Tên đăng nhập | Email | Ghi chú |
|---|---|---|---|
| Admin | `admin` | admin@trasaykho.vn | Quản trị viên hệ thống |
| Nhân viên | `nhanvien01` | nhanvien01@trasaykho.vn | Nhân viên bán hàng |
| Khách hàng | `khachhang01` | khachhang01@gmail.com | Có sẵn 1 đơn hàng + 1 đánh giá demo |
| Khách hàng | `khachhang02` | khachhang02@gmail.com | Chưa có đơn hàng |

> ⚠️ Mật khẩu trong dữ liệu mẫu hiện là chuỗi giả lập, chưa dùng để đăng nhập thật được — sẽ cập nhật khi hoàn thiện chức năng Authentication.

## 📋 Danh sách API chính

| Module | Endpoint | Ghi chú |
|---|---|---|
| Sản phẩm | `GET/POST/PUT/DELETE /api/SanPham` | Đầy đủ CRUD, xóa mềm |
| Danh mục | `GET/POST/PUT/DELETE /api/DanhMuc` | Đầy đủ CRUD, xóa mềm |
| Khuyến mãi | `GET/POST/PUT/DELETE /api/KhuyenMai` | Đầy đủ CRUD, xóa mềm |
| Đơn hàng | `GET /api/DonHang`, `PUT /api/DonHang/{id}/trangthai` | Chỉ cập nhật trạng thái, không sửa/xóa toàn bộ đơn |
| Khách hàng | `GET/PUT /api/KhachHang`, `PUT /api/KhachHang/{id}/trangthai` | Không tạo mới (khách tự đăng ký qua app), có khóa/mở khóa tài khoản |
| Đánh giá | `GET/DELETE /api/DanhGia` | Chỉ đọc và xóa (kiểm duyệt), không tạo/sửa thay khách hàng |
| Thống kê | `GET /api/ThongKe/tongquan`, `GET /api/ThongKe/doanhthu`, `GET /api/ThongKe/sanphambanchay` | Báo cáo doanh thu theo ngày, top sản phẩm bán chạy |
| Hình ảnh sản phẩm | `GET/POST/DELETE /api/SanPham/{sanPhamId}/HinhAnhSanPham` | Upload/xóa ảnh, gắn theo từng sản phẩm |
| Thông báo | `GET/POST /api/ThongBao` | Gửi thông báo cho 1 khách hàng hoặc toàn bộ khách hàng |
| Chatbot AI | `POST /api/Chatbot/chat`, `GET /api/Chatbot/lichsu/{khachHangId}` | Tư vấn sản phẩm bằng AI (Gemini), tự động lưu lịch sử hội thoại |

> Xem chi tiết đầy đủ từng endpoint tại Swagger UI sau khi chạy ứng dụng.

## 📌 Khắc phục sự cố thường gặp

- **Lỗi `Could not find any project in ...`**: Đang đứng sai thư mục khi chạy lệnh `dotnet`. Chạy `cd Backend/TraSayKho.API` trước khi chạy các lệnh `dotnet`.
- **Lỗi `The server was not found or was not accessible`**: Sai tên server trong connection string, hoặc dùng nhầm `/` thay vì `\`. Kiểm tra lại tên server bằng SSMS, dùng đúng `\\` trong file JSON.
- **Lỗi `does not contain a definition for 'XxxYyy'`**: Sai tên `DbSet` khi gọi `_context.XxxYyy`. Mở file `Data/TraSayKhoDbContext.cs`, đối chiếu đúng tên `DbSet` tương ứng.
- **Lỗi `Cannot implicitly convert type 'X?' to 'X'`**: Thiếu xử lý giá trị null (thường gặp ở cột kiểu `date` hoặc computed column). Thêm `?? giá_trị_mặc_định` vào cuối dòng gán.
- **Lỗi `Operator '??' cannot be applied to operands of type 'X' and 'X'`**: Property không phải kiểu nullable nhưng lại dùng `??`. Xóa `?? giá_trị_mặc_định` đi.
- **Máy mỗi người có cấu hình SQL Server khác nhau** (SQLEXPRESS vs MSSQLSERVER, tên instance khác nhau): luôn tự kiểm tra và sửa lại `appsettings.json` theo máy mình, không copy nguyên giá trị của người khác.
- **Chatbot báo lỗi "thiếu API key"**: Chưa cấu hình `user-secrets` theo Bước 3. Chạy lại `dotnet user-secrets set "GeminiApi:ApiKey" "..."` tại đúng thư mục `Backend/TraSayKho.API`.
- **Chatbot báo lỗi mã 404 khi gọi AI**: Model Gemini đang cấu hình trong `appsettings.json` (`GeminiApi:Model`) đã bị Google ngừng hỗ trợ. Kiểm tra nội dung lỗi trả về (Google thường tự gợi ý tên model mới ngay trong thông báo lỗi) rồi cập nhật lại giá trị `Model` cho đúng.