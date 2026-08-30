# 🍵 Ứng dụng bán trà sấy khô tích hợp Chatbot AI (Mô hình đa chi nhánh)

Đây là dự án Web quản trị bán trà sấy khô được xây dựng bằng **ASP.NET Core Web API**, **Entity Framework Core**, và **SQL Server**, theo kiến trúc **N-Layer** (Controller → Service → Repository). Hệ thống hỗ trợ quản lý **nhiều chi nhánh**, theo dõi tồn kho theo **lô hàng** (hạn sử dụng, FEFO), và tích hợp **Chatbot AI** tư vấn sản phẩm.

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

> ⚠️ **Lưu ý:** Mỗi máy có tên SQL Server khác nhau (ví dụ `KHAINGUYEN\SQLEXPRESS`, `TENMAY-PC\SQLEXPRESS`...). Bạn **CẦN PHẢI** sửa lại phần này — tìm tên server của mình bằng cách mở SSMS, xem ở ô "Server name" lúc đăng nhập.

```json
"ConnectionStrings": {
  "TraSayKhoDB": "Server=TEN_SERVER_CUA_BAN\\SQLEXPRESS;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Nếu tên server có dấu `\`, trong file JSON phải gõ **2 dấu `\\`** liền nhau (do quy tắc escape của JSON).

### Bước 3: Cấu hình API Key cho Chatbot AI

```bash
cd Backend/TraSayKho.API
dotnet user-secrets init
dotnet user-secrets set "GeminiApi:ApiKey" "API_KEY_CUA_BAN"
```

> Lấy API key miễn phí tại: https://aistudio.google.com/apikey (đăng nhập Google, không cần thẻ thanh toán). Có thể dùng chung 1 key cho cả nhóm — liên hệ Khải để lấy key nếu chưa có.

> ⚠️ **Không đặt API key trực tiếp trong `appsettings.json`** — lệnh `user-secrets` lưu key ở nơi an toàn ngoài project, tránh lộ khi push Git. Nếu bỏ qua bước này, hệ thống vẫn chạy bình thường — chỉ riêng Chatbot báo lỗi "thiếu API key".

### Bước 4: Khởi động ứng dụng

```bash
dotnet restore
dotnet run
```

Mở trình duyệt vào `http://localhost:{port}/swagger` (port hiển thị trong Terminal, dòng `Now listening on...`).

### Bước 5: Chạy lại dữ liệu mẫu (Không bắt buộc — Chỉ dùng làm dự phòng)

Nếu muốn đặt lại dữ liệu gốc từ đầu: xóa database `TraSayKhoDB` cũ trong SSMS (chuột phải → Delete), rồi chạy lại toàn bộ file `Database/Database_TraSayKho.sql` như Bước 1.

## 🌐 Trải nghiệm ứng dụng

- **Swagger API:** `http://localhost:{port}/swagger`

### Chi nhánh mẫu

| Chi nhánh | Địa chỉ | Ghi chú |
|---|---|---|
| Chi nhánh Quận 1 | 123 Nguyễn Huệ, Quận 1, TP.HCM | Chi nhánh chính (trụ sở) |
| Chi nhánh Thủ Đức | 45 Võ Văn Ngân, Thủ Đức, TP.HCM | Chi nhánh phụ |

### Tài khoản đăng nhập mẫu

| Vai trò | Tên đăng nhập | Email | Ghi chú |
|---|---|---|---|
| Admin tổng | `admin` | admin@trasaykho.vn | Không gắn chi nhánh, xem toàn hệ thống |
| Quản lý chi nhánh | `nhanvien01` | nhanvien01@trasaykho.vn | Thuộc Chi nhánh Quận 1 |
| Quản lý chi nhánh | `nhanvien02` | nhanvien02@trasaykho.vn | Thuộc Chi nhánh Thủ Đức |
| Khách hàng | `khachhang01` | khachhang01@gmail.com | Có sẵn 1 đơn hàng + 1 đánh giá demo |
| Khách hàng | `khachhang02` | khachhang02@gmail.com | Chưa có đơn hàng |

> ⚠️ Mật khẩu trong dữ liệu mẫu hiện là chuỗi giả lập, chưa dùng để đăng nhập thật được — sẽ cập nhật khi hoàn thiện chức năng Authentication.

## 📋 Danh sách API chính

| Module | Endpoint | Ghi chú |
|---|---|---|
| Sản phẩm | `GET/POST/PUT/DELETE /api/SanPham` | Đầy đủ CRUD, xóa mềm. `SoLuongTon`/`HanSuDung` tự đồng bộ từ Lô hàng |
| Danh mục | `GET/POST/PUT/DELETE /api/DanhMuc` | Đầy đủ CRUD, xóa mềm |
| Khuyến mãi | `GET/POST/PUT/DELETE /api/KhuyenMai` | Đầy đủ CRUD, xóa mềm |
| Đơn hàng | `GET /api/DonHang`, `PUT /api/DonHang/{id}/trangthai` | Chỉ cập nhật trạng thái theo đúng thứ tự, gắn với Chi nhánh xử lý |
| Khách hàng | `GET/PUT /api/KhachHang`, `PUT /api/KhachHang/{id}/trangthai` | Không tạo mới (khách tự đăng ký qua app), có khóa/mở khóa tài khoản |
| Đánh giá | `GET/DELETE /api/DanhGia` | Chỉ đọc và xóa (kiểm duyệt) |
| **Chi nhánh** | `GET/POST/PUT/DELETE /api/ChiNhanh` | Quản lý danh sách chi nhánh, xóa mềm |
| **Lô hàng** | `GET/POST /api/LoHang`, `GET /api/LoHang/sanpham/{id}`, `GET /api/LoHang/saphethan?soNgay=30` | Nhập lô mới (tự đồng bộ tồn kho sản phẩm), tra cứu theo sản phẩm, cảnh báo cận hạn |
| **Xả kho (theo lô)** | `PUT /api/LoHang/{id}/xakho`, `PUT /api/LoHang/{id}/huyxakho` | Gán/hủy mức giảm giá riêng cho từng lô cận hạn |
| **Phiếu điều chuyển kho** | `GET/POST /api/PhieuDieuChuyen`, `PUT /api/PhieuDieuChuyen/{id}/xacnhan`, `PUT /api/PhieuDieuChuyen/{id}/huy` | Chuyển hàng giữa 2 chi nhánh, giữ nguyên lô/hạn sử dụng khi chuyển |
| Thống kê | `GET /api/ThongKe/tongquan`, `.../doanhthu`, `.../sanphambanchay` | Thêm tham số `?chiNhanhId=` tùy chọn — bỏ trống để xem toàn hệ thống, truyền vào để xem riêng 1 chi nhánh |
| Hình ảnh sản phẩm | `GET/POST/DELETE /api/SanPham/{sanPhamId}/HinhAnhSanPham` | Upload/xóa ảnh, gắn theo từng sản phẩm |
| Thông báo | `GET/POST /api/ThongBao` | Gửi thông báo cho 1 khách hàng hoặc toàn bộ khách hàng |
| Chatbot AI | `POST /api/Chatbot/chat`, `GET /api/Chatbot/lichsu/{khachHangId}`, `PUT /api/Chatbot/dongphien/{cuocHoiThoaiId}` | Tư vấn sản phẩm bằng AI (Gemini), tự lưu lịch sử, tự đóng phiên sau 60 phút không hoạt động |

> Xem chi tiết đầy đủ từng endpoint (tham số, mẫu request/response) tại Swagger UI sau khi chạy ứng dụng.

## 📌 Khắc phục sự cố thường gặp

- **Lỗi `Could not find any project in ...`**: Đang đứng sai thư mục khi chạy lệnh `dotnet`. Chạy `cd Backend/TraSayKho.API` trước khi chạy các lệnh `dotnet`.
- **Lỗi `The server was not found or was not accessible`**: Sai tên server trong connection string, hoặc dùng nhầm `/` thay vì `\`. Kiểm tra lại bằng SSMS, dùng đúng `\\` trong file JSON.
- **Lỗi `does not contain a definition for 'XxxYyy'`**: Sai tên `DbSet` khi gọi `_context.XxxYyy`. Mở `Data/TraSayKhoDbContext.cs`, đối chiếu đúng tên `DbSet` tương ứng.
- **Lỗi `Cannot implicitly convert type 'X?' to 'X'`**: Thiếu xử lý giá trị null (thường gặp ở cột `date` hoặc computed column). Thêm `?? giá_trị_mặc_định` vào cuối dòng gán.
- **Lỗi `Operator '??' cannot be applied to operands of type 'X' and 'X'`**: Property không phải kiểu nullable nhưng lại dùng `??`. Xóa `?? giá_trị_mặc_định` đi.
- **Chatbot báo lỗi "thiếu API key"**: Chưa cấu hình `user-secrets` theo Bước 3.
- **Chatbot báo lỗi mã 404 khi gọi AI**: Model Gemini đang cấu hình (`GeminiApi:Model` trong `appsettings.json`) đã bị Google ngừng hỗ trợ. Đọc nội dung lỗi trả về (Google thường tự gợi ý tên model mới ngay trong thông báo lỗi) rồi cập nhật lại giá trị `Model`.
- **Sau khi pull code có thay đổi database (thêm bảng/cột mới)**: cần chạy lại đúng script `.sql` mới nhất (Bước 1), và scaffold lại Model:
```bash
  cd Backend/TraSayKho.API
  dotnet ef dbcontext scaffold "Server=TEN_SERVER;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context TraSayKhoDbContext --context-dir Data --no-onconfiguring --force
```
  Tham số `--force` cho phép ghi đè Model cũ — an toàn vì các file trong `Models/`, `Data/` không được sửa tay trực tiếp.
- **Máy mỗi người có cấu hình SQL Server khác nhau**: luôn tự kiểm tra và sửa lại `appsettings.json` theo máy mình, không copy nguyên giá trị của người khác.