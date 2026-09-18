# 🍵 Trà Sấy Khô — Hệ thống quản lý bán hàng đa chi nhánh

> Ứng dụng Web quản trị tích hợp **Chatbot AI**, xây dựng bằng **ASP.NET Core Web API** · **Entity Framework Core** · **SQL Server**

---

## Tổng quan

Hệ thống hỗ trợ quản lý **nhiều chi nhánh** với các tính năng:

- 📦 Tồn kho theo lô hàng / FEFO, xả kho đa bậc tự động, điều chuyển kho 4 bước
- 🤖 **Chatbot AI** (Gemini) tư vấn sản phẩm
- 🛒 **Giỏ hàng + Đặt hàng** với trừ kho thật
- 🔐 **JWT Authentication** với 4 vai trò phân quyền

Kiến trúc **N-Layer**: Controller → Service → Repository

---

## Yêu cầu hệ thống

| Thành phần | Yêu cầu |
|---|---|
| .NET SDK | 8.0 trở lên |
| Cơ sở dữ liệu | Microsoft SQL Server 2019+ (kèm SSMS) |
| IDE khuyên dùng | Visual Studio Code + extension C# Dev Kit |

---

## Hướng dẫn cài đặt

### Bước 1 — Chuẩn bị cơ sở dữ liệu

1. Mở **SQL Server Management Studio (SSMS)** và đăng nhập (Windows Authentication).
2. Mở **New Query**, copy toàn bộ nội dung file `Database/Database_TraSayKho.sql`, dán vào và **Execute (F5)**.

> ✅ **Không cần chạy thêm lệnh nào khác** — script đã bao gồm sẵn: 2 chi nhánh mẫu, 4 vai trò, 3 bậc giảm giá xả kho, danh mục, sản phẩm, lô hàng theo từng chi nhánh, tài khoản thử nghiệm, và 1 đơn hàng demo.

---

### Bước 2 — Cấu hình kết nối database

Mở file `Backend/TraSayKho.API/appsettings.json` và sửa connection string:

```json
"ConnectionStrings": {
  "TraSayKhoDB": "Server=TEN_SERVER_CUA_BAN\\SQLEXPRESS;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> ⚠️ Mỗi máy có tên SQL Server khác nhau (ví dụ `KHAINGUYEN\SQLEXPRESS`, `TENMAY-PC\SQLEXPRESS`). Tìm tên server của mình tại ô **"Server name"** khi đăng nhập SSMS.
>
> Nếu tên server có dấu `\`, trong JSON phải gõ **2 dấu `\\`** liền nhau (quy tắc escape của JSON).

---

### Bước 3 — Cấu hình khóa bí mật (Secrets)

Mở Terminal tại `Backend/TraSayKho.API`, chạy lần lượt:

```bash
dotnet user-secrets init
dotnet user-secrets set "GeminiApi:ApiKey" "API_KEY_GEMINI_CUA_BAN"
dotnet user-secrets set "JwtSettings:SecretKey" "day-la-chuoi-bi-mat-rat-dai-va-kho-doan-cho-du-an-tra-say-kho-2026"
```

- **Gemini API key**: lấy miễn phí tại https://aistudio.google.com/apikey (đăng nhập Google, không cần thẻ thanh toán).
- **JWT SecretKey**: chuỗi bí mật ký/xác minh token đăng nhập, nên dài ≥ 32 ký tự.

> ⚠️ **Không đặt các giá trị này trực tiếp trong `appsettings.json`** — lệnh `user-secrets` lưu ở nơi an toàn ngoài project (`%APPDATA%\Microsoft\UserSecrets\` trên Windows), tránh bị lộ khi push Git.

Thêm phần cấu hình không nhạy cảm vào `appsettings.json`:

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

> `ExpiryMinutes: 1440` = token có hiệu lực **24 giờ**, sau đó cần đăng nhập lại.

---

### Bước 4 — Khởi động ứng dụng

```bash
dotnet restore
dotnet run
```

Mở trình duyệt vào `http://localhost:{port}/swagger` (port hiển thị trong Terminal, dòng `Now listening on...`).

---

### Bước 5 — Chạy lại dữ liệu mẫu *(tuỳ chọn — chỉ dùng làm dự phòng)*

1. Xóa database `TraSayKhoDB` cũ trong SSMS (chuột phải → Delete).
2. Chạy lại toàn bộ file `Database/Database_TraSayKho.sql` như Bước 1.

---

## Đăng nhập & Phân quyền

### Cách lấy Token trên Swagger

1. `POST /api/Auth/dangnhap` → copy giá trị `token` trong response.
2. Bấm nút **Authorize 🔒** ở đầu trang Swagger.
3. Trong ô Value, **chỉ dán đúng chuỗi token** — **KHÔNG gõ thêm chữ "Bearer"** (Swagger tự thêm sẵn, gõ thêm sẽ bị lặp "Bearer Bearer" gây lỗi 401).
4. Bấm **Authorize** → thấy chữ "Authorized" → bấm **Close**.

> ⚠️ Token mới chứa thêm `KhachHangId` hoặc `NhanVienId`. **Đăng nhập lại để lấy token mới** sau khi cập nhật phiên bản phân quyền — token cũ có thể nhận lỗi `403 Forbidden` khi gọi giỏ hàng, đơn hàng, chatbot, đánh giá hoặc thông báo cá nhân.
>
> ⚠️ **Tài khoản mẫu trong dữ liệu mẫu (`admin`, `nhanvien01`, `khachhang01`...) KHÔNG đăng nhập được** — mật khẩu chưa qua mã hóa BCrypt thật. Cần **tạo tài khoản mới**:
> - Khách hàng: `POST /api/Auth/dangky` (mở tự do).
> - Quản trị / Chủ cửa hàng / Nhân viên: `POST /api/Auth/taonhanvien` — **chỉ Admin đã đăng nhập** mới gọi được (tài khoản Admin đầu tiên cần tạo trực tiếp trong SQL).

---

### Bảng phân quyền — 4 vai trò

| Vai trò | Phạm vi | Công việc chính |
|---|---|---|
| **Quản trị hệ thống** (`Admin`) | Toàn hệ thống (`ChiNhanhId = null`) | Quản lý chi nhánh, tạo tài khoản nội bộ, xem báo cáo toàn hệ thống, cấu hình bậc giảm giá xả kho |
| **Chủ cửa hàng** (`ChuCuaHang`) | 1 chi nhánh cụ thể | Quản lý lô hàng & HSD, khuyến mãi, tạo/duyệt/từ chối phiếu điều chuyển, báo cáo chi nhánh, khóa/mở tài khoản khách |
| **Nhân viên** (`NhanVien`) | 1 chi nhánh cụ thể | Điều chỉnh tồn kho vận hành, xác nhận/cập nhật trạng thái đơn hàng, thực hiện xuất/nhận hàng điều chuyển |
| **Khách hàng** (`KhachHang`) | Cá nhân | Tìm kiếm sản phẩm, chatbot, giỏ hàng, đặt hàng, đánh giá đơn hoàn thành, xem thông báo |

> **Lưu ý phân biệt:** Chủ cửa hàng phụ trách quyết định/thiết lập (nhập lô, tạo khuyến mãi, duyệt điều chuyển). Nhân viên phụ trách vận hành thực tế (xuất/nhận hàng thật, xác nhận đơn). Cả 2 bị giới hạn theo đúng 1 chi nhánh — khác nhau ở **loại công việc**, không phải phạm vi.

### Chi nhánh mẫu

| Chi nhánh | Địa chỉ | Ghi chú |
|---|---|---|
| Chi nhánh Quận 1 | 123 Nguyễn Huệ, Quận 1, TP.HCM | Chi nhánh chính (trụ sở) |
| Chi nhánh Thủ Đức | 45 Võ Văn Ngân, Thủ Đức, TP.HCM | Chi nhánh phụ |

---

## Quy trình điều chuyển kho (4 bước)

Khi 1 chi nhánh thiếu hàng, có thể xin điều chuyển từ chi nhánh khác đang còn tồn:

```
ChoDuyet → DaDuyet (hoặc TuChoi) → DangVanChuyen → HoanTat
```

| Bước | Endpoint | Người thực hiện |
|---|---|---|
| 1. Tạo yêu cầu | `POST /api/PhieuDieuChuyen` | Chủ cửa hàng chi nhánh **thiếu hàng** |
| 2. Duyệt / Từ chối | `PUT .../{id}/duyet` hoặc `.../tuchoi` | Chủ cửa hàng chi nhánh **nguồn** |
| 3. Xuất kho | `PUT .../{id}/xuatkho` | Nhân viên chi nhánh **nguồn** → trừ kho tại đây |
| 4. Nhận hàng | `PUT .../{id}/nhanhang` | Nhân viên chi nhánh **đích** → cộng kho tại đây |

---

## Cơ chế khuyến mãi (2 loại song song)

| | `KhuyenMai` | Xả kho theo Lô |
|---|---|---|
| **Mục đích** | Marketing chung (lễ, Tết, khai trương...) | Xử lý hàng tồn **sắp hết hạn** |
| **Cách áp dụng** | Khách tự nhập **mã code** lúc thanh toán | **Tự động** hiển thị giá đã giảm, khách không cần làm gì |
| **Phạm vi** | Toàn đơn hàng (giảm trên tổng tiền) | Riêng 1 lô hàng cụ thể |
| **Ai quản lý** | Admin, Chủ cửa hàng | Chủ cửa hàng (thủ công) hoặc hệ thống (tự động) |

**Xả kho hoạt động theo 2 tầng ưu tiên** (tính lại mỗi lần truy vấn, không cần tiến trình nền):

1. Nếu Chủ cửa hàng đã **bấm tay** (`PUT /api/LoHang/{id}/xakho`) và còn trong khoảng ngày hiệu lực → dùng đúng mức đó, **không bị ghi đè**.
2. Nếu không có ai can thiệp tay → hệ thống **tự động** tra bảng `BacGiamGiaXaKho`:

| Còn lại | Giảm giá |
|---|---|
| ≤ 90 ngày | 15% |
| ≤ 30 ngày | 30% |
| ≤ 7 ngày | 50% |

> **Lưu ý:** Khi **tạo đơn hàng thật** (`POST /api/DonHang`), hệ thống chỉ tôn trọng mức giảm **thủ công** đã xác nhận — tránh rủi ro giá biến động ngay lúc chốt đơn.

---

## Giỏ hàng & Đặt hàng (FEFO thật)

**Giỏ hàng** (`/api/GioHang`): yêu cầu đăng nhập với vai trò `KhachHang`. Hệ thống lấy `KhachHangId` từ JWT và kiểm tra quyền sở hữu trước khi xem, cập nhật hoặc xóa chi tiết giỏ.

**Đặt hàng** (`POST /api/DonHang`): Backend lấy `KhachHangId` từ JWT — không tin ID do client gửi trong body. Với mỗi sản phẩm trong đơn, hệ thống tự động:

1. Tìm lô có hạn sử dụng **gần nhất** mà **tự nó đủ** số lượng đặt (không chia nhỏ qua nhiều lô — nếu không lô nào đủ, báo lỗi rõ ràng kèm tổng tồn kho hiện có).
2. Áp đúng giá đã giảm (nếu lô đó đang xả kho thủ công).
3. Áp mã khuyến mãi nếu có (kiểm tra hiệu lực, số lượt còn lại, giá trị đơn tối thiểu).
4. Trừ kho thật ngay tại lô đã chọn, đồng bộ lại tồn kho tổng của sản phẩm.
5. Toàn bộ 5 bước nằm trong **1 luồng lưu duy nhất** — nếu lỗi giữa chừng, mọi thay đổi được hủy bỏ đồng thời (không có chuyện "trừ kho rồi nhưng đơn không tạo").

---

## Danh sách API đầy đủ

### Xác thực

| Method | Endpoint | Quyền |
|---|---|---|
| POST | `/api/Auth/dangky` | Mở (khách hàng tự đăng ký) |
| POST | `/api/Auth/taonhanvien` | Chỉ Admin |
| POST | `/api/Auth/dangnhap` | Mở |

### Sản phẩm · Danh mục · Khuyến mãi · Đánh giá

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/SanPham`, `/api/SanPham/{id}` | Công khai |
| POST/PUT/DELETE | `/api/SanPham` | Admin, ChuCuaHang (xóa mềm) |
| GET | `/api/DanhMuc`, `/api/DanhMuc/{id}` | Công khai |
| POST/PUT/DELETE | `/api/DanhMuc` | Admin, ChuCuaHang |
| GET | `/api/KhuyenMai` | Admin, NhanVien, ChuCuaHang |
| POST/PUT/DELETE | `/api/KhuyenMai` | Admin, ChuCuaHang |
| GET | `/api/DanhGia`, `/api/DanhGia/{id}` | Công khai |
| POST | `/api/DanhGia` | KhachHang (chỉ đơn của mình đã `HoanThanh`) |
| DELETE | `/api/DanhGia/{id}` | Admin, ChuCuaHang |

### Chi nhánh

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/ChiNhanh`, `/{id}` | Admin, NhanVien, ChuCuaHang |
| POST/PUT/DELETE | `/api/ChiNhanh` | Chỉ Admin |

### Lô hàng

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/LoHang`, `.../{id}`, `.../sanpham/{id}`, `.../saphethan?soNgay=30` | Xem theo chi nhánh (Admin xem hết) |
| POST | `/api/LoHang` | Admin, ChuCuaHang |
| PUT | `.../{id}/xakho`, `.../huyxakho` | Admin, ChuCuaHang |
| PUT | `.../{id}/dieuchinhton` | Admin, NhanVien |

### Bậc giảm giá xả kho

| Method | Endpoint | Quyền |
|---|---|---|
| GET/POST/PUT/DELETE | `/api/BacGiamGia` | Chỉ Admin |

### Đơn hàng

| Method | Endpoint | Quyền |
|---|---|---|
| POST | `/api/DonHang` | KhachHang; ID lấy từ JWT |
| GET | `/api/DonHang`, `.../{id}` | Admin, NhanVien, ChuCuaHang (theo chi nhánh) |
| PUT | `.../{id}/trangthai` | Admin, NhanVien, ChuCuaHang (theo chi nhánh) |

### Giỏ hàng

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/GioHang` | KhachHang (giỏ của tài khoản đang đăng nhập) |
| POST | `/api/GioHang/them` | KhachHang |
| PUT/DELETE | `.../chitiet/{chiTietId}` | KhachHang (phải sở hữu chi tiết giỏ) |
| DELETE | `/api/GioHang/xoahet` | KhachHang |

> Các endpoint cũ có `{khachHangId}` vẫn hoạt động để tương thích, nhưng chỉ khi ID trên URL trùng với `KhachHangId` trong JWT.

### Phiếu điều chuyển kho

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/PhieuDieuChuyen`, `.../{id}` | Xem theo chi nhánh liên quan |
| POST | `/api/PhieuDieuChuyen` | Admin, ChuCuaHang (chi nhánh nhận) |
| PUT | `.../{id}/duyet`, `.../tuchoi` | Admin, ChuCuaHang (chi nhánh nguồn) |
| PUT | `.../{id}/xuatkho` | Admin, NhanVien (chi nhánh nguồn) |
| PUT | `.../{id}/nhanhang` | Admin, NhanVien (chi nhánh đích) |

### Khách hàng

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/KhachHang`, `.../{id}` | Admin, NhanVien, ChuCuaHang |
| PUT | `.../{id}`, `.../{id}/trangthai` | Admin, ChuCuaHang |

### Thống kê

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/ThongKe/tongquan`, `.../doanhthu`, `.../sanphambanchay` | Admin: toàn hệ thống; NhanVien/ChuCuaHang: bị ép về chi nhánh mình |

### Hình ảnh sản phẩm · Thông báo

| Method | Endpoint | Quyền |
|---|---|---|
| GET | `/api/SanPham/{id}/HinhAnhSanPham` | Công khai |
| POST/DELETE | `/api/SanPham/{id}/HinhAnhSanPham` | Admin, ChuCuaHang |
| GET/POST | `/api/ThongBao` | Admin, NhanVien, ChuCuaHang |
| GET | `/api/ThongBao/cuatoi` | KhachHang (chỉ thông báo của mình) |
| GET | `/api/ThongBao/khachhang/{khachHangId}` | Nội bộ; KhachHang chỉ dùng đúng ID của mình |

### Chatbot AI

| Method | Endpoint | Quyền |
|---|---|---|
| POST | `/api/Chatbot/chat` | KhachHang; ID lấy từ JWT |
| GET | `/api/Chatbot/lichsu/{khachHangId}` | KhachHang (chỉ lịch sử của mình) |
| PUT | `/api/Chatbot/dongphien/{cuocHoiThoaiId}` | KhachHang (chỉ phiên của mình) |

> Chi tiết đầy đủ tham số, mẫu request/response tại **Swagger UI** sau khi chạy ứng dụng.

---

## Khắc phục sự cố thường gặp

| Lỗi | Nguyên nhân & Cách xử lý |
|---|---|
| `Could not find any project in ...` | Đang đứng sai thư mục. Chạy `cd Backend/TraSayKho.API` trước. |
| `The server was not found or was not accessible` | Sai tên server trong connection string, hoặc dùng nhầm `/` thay vì `\`. Kiểm tra lại bằng SSMS, dùng đúng `\\` trong JSON. |
| `does not contain a definition for 'XxxYyy'` | Sai tên `DbSet`. Mở `Data/TraSayKhoDbContext.cs`, đối chiếu đúng tên `DbSet` (EF Core scaffold đôi khi đặt tên số ít/nhiều khác dự đoán). |
| `Cannot implicitly convert type 'X?' to 'X'` | Thiếu xử lý null (thường ở cột `date` hoặc computed column). Thêm `?? giá_trị_mặc_định` vào cuối dòng gán. |
| `Operator '??' cannot be applied to operands of type 'X' and 'X'` | Property không phải kiểu nullable nhưng dùng `??`. Xóa `?? giá_trị_mặc_định` đi. |
| `The type or namespace name 'XxxDto' could not be found` | File DTO bị copy sót đoạn cuối. Kiểm tra và dán lại toàn bộ file DTO liên quan. |
| Chatbot báo lỗi "thiếu API key" | Chưa cấu hình `user-secrets` theo Bước 3. |
| Chatbot báo lỗi mã 404 khi gọi AI | Model Gemini đang cấu hình đã bị Google ngừng hỗ trợ. Đọc nội dung lỗi trả về (Google tự gợi ý model mới), cập nhật lại `GeminiApi:Model`. |
| `401 Unauthorized` | Chưa đăng nhập / chưa Authorize trên Swagger, hoặc token hết hạn (24h). Đăng nhập lại lấy token mới. Restart server cũng làm Swagger mất trạng thái Authorize — bấm Authorize lại là được. |
| `403 Forbidden` | Đã đăng nhập nhưng sai vai trò, cố thao tác ngoài chi nhánh, truy cập dữ liệu của khách khác, hoặc đang dùng token cũ chưa có `KhachHangId`/`NhanVienId`. Đăng nhập lại lấy token mới. |
| Nhập token bị lặp "Bearer Bearer" | Trong ô Value của Swagger Authorize, **chỉ dán chuỗi token thuần**, không tự gõ thêm chữ "Bearer". |
| Mỗi máy cấu hình SQL Server khác nhau | Luôn tự kiểm tra và sửa lại `appsettings.json` theo máy mình, không copy nguyên giá trị của người khác. |

### Scaffold lại Model sau khi đổi database

Nếu thêm bảng/cột mới, chạy lại script `.sql` mới nhất rồi scaffold lại:

```bash
cd Backend/TraSayKho.API
dotnet ef dbcontext scaffold "Server=TEN_SERVER;Database=TraSayKhoDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context TraSayKhoDbContext --context-dir Data --no-onconfiguring --force
```

> `--force` cho phép ghi đè Model cũ — an toàn vì các file trong `Models/`, `Data/` không được sửa tay trực tiếp.
