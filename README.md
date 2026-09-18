**# 🍵 Ứng dụng bán trà sấy khô tích hợp Chatbot AI (Mô hình đa chi nhánh)**

Đây là dự án Web quản trị bán trà sấy khô được xây dựng bằng **\*\*ASP.NET Core Web API\*\***, **\*\*Entity Framework Core\*\***, và **\*\*SQL Server\*\***, theo kiến trúc **\*\*N-Layer\*\*** (Controller → Service → Repository). Hệ thống hỗ trợ quản lý **\*\*nhiều chi nhánh\*\*** (tồn kho theo lô hàng/FEFO, xả kho đa bậc tự động, điều chuyển kho 4 bước), tích hợp **\*\*Chatbot AI\*\*** (Gemini) tư vấn sản phẩm, **\*\*Giỏ hàng + Đặt hàng\*\*** với trừ kho thật, và bảo mật bằng **\*\*JWT Authentication\*\*** với **\*\*4 vai trò phân quyền\*\***: Quản trị hệ thống, Chủ cửa hàng, Nhân viên, Khách hàng.

**## 🛠️ Yêu cầu hệ thống**

\- **\*\*.NET SDK\*\***: 8.0 trở lên

\- **\*\*Cơ sở dữ liệu\*\***: Microsoft SQL Server (2019 hoặc mới hơn), kèm SSMS

\- **\*\*IDE khuyên dùng\*\***: Visual Studio Code (kèm extension C# Dev Kit)

**## 🚀 Hướng dẫn cài đặt và chạy ứng dụng**

**### Bước 1: Chuẩn bị Cơ sở dữ liệu (Database)**

1\. Mở **\*\*SQL Server Management Studio (SSMS)\*\***.

2\. Đăng nhập vào SQL Server (Windows Authentication).

3\. Mở **\*\*New Query\*\***, copy toàn bộ nội dung file \`Database/Database\_TraSayKho.sql\`, dán vào và **\*\*Execute (F5)\*\*** để tạo database, các bảng và dữ liệu mẫu.

\> ✅ **\*\*Không cần chạy thêm lệnh nào khác\*\*** — script đã bao gồm sẵn: 2 chi nhánh mẫu, 4 vai trò (Admin, ChuCuaHang, NhanVien, KhachHang), 3 bậc giảm giá xả kho, danh mục, sản phẩm, lô hàng theo từng chi nhánh, tài khoản thử nghiệm, và 1 đơn hàng demo.

**### Bước 2: Cấu hình kết nối Database**

Mở file \`Backend/TraSayKho.API/appsettings.json\` và sửa lại thông tin cấu hình SQL Server cho phù hợp với máy của bạn.

\> ⚠️ **\*\*Lưu ý:\*\*** Mỗi máy có tên SQL Server khác nhau (ví dụ \`KHAINGUYEN\SQLEXPRESS\`, \`TENMAY-PC\SQLEXPRESS\`...). Bạn **\*\*CẦN PHẢI\*\*** sửa lại phần này — tìm tên server của mình bằng cách mở SSMS, xem ở ô "Server name" lúc đăng nhập.

Ví dụ cấu hình dùng Windows Authentication:

\`\`\`json

"ConnectionStrings": {

  "TraSayKhoDB": "Server=TEN\_SERVER\_CUA\_BAN\\\SQLEXPRESS;Database=TraSayKhoDB;Trusted\_Connection=True;TrustServerCertificate=True;"

}

\`\`\`

\> Nếu tên server có dấu \`\\\`, trong file JSON phải gõ **\*\*2 dấu \`\\\\\`\*\*** liền nhau (do quy tắc escape của JSON).

**### Bước 3: Cấu hình các khóa bí mật (Secrets)**

Mở Terminal tại \`Backend/TraSayKho.API\`, chạy lần lượt:

\`\`\`bash

dotnet user-secrets init

dotnet user-secrets set "GeminiApi\:ApiKey" "API\_KEY\_GEMINI\_CUA\_BAN"

dotnet user-secrets set "JwtSettings\:SecretKey" "day-la-chuoi-bi-mat-rat-dai-va-kho-doan-cho-du-an-tra-say-kho-2026"

\`\`\`

\> - **\*\*Gemini API key\*\***: lấy miễn phí tại https\://aistudio.google.com/apikey (đăng nhập Google, không cần thẻ thanh toán). Có thể dùng chung 1 key cho cả nhóm — liên hệ Khải nếu chưa có.

\> - **\*\*JWT SecretKey\*\***: chuỗi bí mật dùng để ký/xác minh token đăng nhập, nên dài (≥32 ký tự), có thể tự nghĩ ra hoặc dùng đúng ví dụ trên cho mục đích đồ án.

\> - ⚠️ **\*\*Không đặt các giá trị này trực tiếp trong \`appsettings.json\`\*\*** — lệnh \`user-secrets\` lưu ở nơi an toàn ngoài project (thường ở \`%APPDATA%\Microsoft\UserSecrets\\\` trên Windows), tránh bị lộ khi push Git.

Mở \`appsettings.json\`, thêm phần cấu hình không nhạy cảm:

\`\`\`json

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

\`\`\`

\> \`ExpiryMinutes: 1440\` nghĩa là token có hiệu lực 24 giờ — sau đó cần đăng nhập lại.

**### Bước 4: Khởi động ứng dụng**

Mở Terminal tại thư mục \`Backend/TraSayKho.API\`, chạy lần lượt:

\`\`\`bash

dotnet restore

dotnet run

\`\`\`

Mở trình duyệt vào \`http\://localhost:{port}/swagger\` (port hiển thị trong Terminal, dòng \`Now listening on...\`).

**### Bước 5: Chạy lại dữ liệu mẫu (Không bắt buộc — Chỉ dùng làm dự phòng)**

Nếu vì lý do nào đó bạn muốn đặt lại dữ liệu gốc từ đầu:

1\. Xóa database \`TraSayKhoDB\` cũ trong SSMS (chuột phải → Delete).

2\. Chạy lại toàn bộ file \`Database/Database\_TraSayKho.sql\` như Bước 1.

**## 🔐 Đăng nhập & Phân quyền (4 vai trò)**

**### Cách lấy Token trên Swagger**

1\. \`POST /api/Auth/dangnhap\` → copy giá trị \`token\` trong response.

2\. Bấm nút **\*\*Authorize 🔒\*\*** ở đầu trang Swagger.

3\. Trong ô Value, **\*\*chỉ dán đúng chuỗi token\*\*** — **\*\*KHÔNG gõ thêm chữ "Bearer"\*\*** (Swagger tự thêm sẵn rồi, gõ thêm sẽ bị lặp "Bearer Bearer" gây lỗi 401).

4\. Bấm **\*\*Authorize\*\*** → thấy chữ "Authorized" → bấm **\*\*Close\*\***.

5\. Từ giờ mọi request đều tự động kèm token.

\> ⚠️ Sau khi cập nhật phiên bản phân quyền mới, tất cả tài khoản cần **\*\*đăng nhập lại để lấy token mới\*\***. Token mới chứa thêm \`KhachHangId\` hoặc \`NhanVienId\` để API xác định đúng chủ sở hữu dữ liệu. Token cũ có thể nhận lỗi \`403 Forbidden\` khi gọi giỏ hàng, đơn hàng, chatbot, đánh giá hoặc thông báo cá nhân.

\> ⚠️ **\*\*Tài khoản mẫu trong dữ liệu mẫu (\`admin\`, \`nhanvien01\`, \`khachhang01\`...) KHÔNG đăng nhập được\*\*** — mật khẩu của chúng là chuỗi giả lập, chưa qua mã hóa BCrypt thật. Cần **\*\*tạo tài khoản mới\*\***:

\> - Khách hàng: \`POST /api/Auth/dangky\` (mở tự do, ai cũng gọi được).

\> - Quản trị hệ thống / Chủ cửa hàng / Nhân viên: \`POST /api/Auth/taonhanvien\` — **\*\*chỉ Admin đã đăng nhập mới gọi được\*\*** API này (tạo tài khoản Admin đầu tiên cần thao tác trực tiếp trong SQL, hoặc nhờ người đã có sẵn quyền Admin tạo giúp).

**### Bảng phân quyền chi tiết theo 4 vai trò**

\| Vai trò | Phạm vi | Công việc chính |

\|---|---|---|

\| **\*\*Quản trị hệ thống\*\*** (\`Admin\`) | Toàn hệ thống, không giới hạn chi nhánh (\`ChiNhanhId = null\` trong token) | Quản lý Chi nhánh (tạo/sửa/xóa), tạo tài khoản nội bộ (Chủ cửa hàng/Nhân viên), xem báo cáo **\*\*toàn hệ thống\*\***, cấu hình Bậc giảm giá xả kho |

\| **\*\*Chủ cửa hàng\*\*** (\`ChuCuaHang\`) | Giới hạn đúng **\*\*1 chi nhánh\*\*** (\`ChiNhanhId\` cụ thể) | Quản lý Lô hàng & HSD (nhập lô mới — thiết lập), Khuyến mãi, **\*\*tạo/duyệt/từ chối\*\*** Phiếu điều chuyển kho, xem báo cáo **\*\*chi nhánh mình\*\***, khóa/mở khóa tài khoản khách hàng |

\| **\*\*Nhân viên\*\*** (\`NhanVien\`) | Giới hạn đúng **\*\*1 chi nhánh\*\*** | Điều chỉnh tồn kho **\*\*vận hành\*\*** hằng ngày (xuất kho giao hàng, kiểm kê — khác với nhập lô mới), xác nhận/cập nhật trạng thái đơn hàng, **\*\*thực hiện thật\*\*** việc xuất/nhận hàng điều chuyển kho, hỗ trợ chatbot khi AI không xử lý được |

\| **\*\*Khách hàng\*\*** (\`KhachHang\`) | Cá nhân | Tìm kiếm sản phẩm, trò chuyện Chatbot, quản lý giỏ hàng, đặt hàng, đánh giá sau khi đơn hoàn thành và xem thông báo của chính mình |

\> Nhớ phân biệt: **\*\*Chủ cửa hàng\*\*** phụ trách các quyết định/thiết lập (nhập lô, tạo khuyến mãi, duyệt điều chuyển), còn **\*\*Nhân viên\*\*** phụ trách thao tác vận hành thực tế hằng ngày (xuất/nhận hàng thật, xác nhận đơn). Cả 2 đều bị giới hạn theo đúng 1 chi nhánh, khác nhau ở **\*\*loại công việc\*\***, không phải ở phạm vi.

**### Chi nhánh mẫu**

\| Chi nhánh | Địa chỉ | Ghi chú |

\|---|---|---|

\| Chi nhánh Quận 1 | 123 Nguyễn Huệ, Quận 1, TP.HCM | Chi nhánh chính (trụ sở) |

\| Chi nhánh Thủ Đức | 45 Võ Văn Ngân, Thủ Đức, TP.HCM | Chi nhánh phụ |

**## 🔄 Quy trình Điều chuyển kho (4 bước)**

Khi 1 chi nhánh thiếu hàng, có thể xin điều chuyển từ chi nhánh khác đang còn tồn, theo đúng 4 bước:

1\. **\*\*Tạo yêu cầu\*\*** (\`POST /api/PhieuDieuChuyen\`) — Chủ cửa hàng chi nhánh **\*\*thiếu hàng\*\*** đề xuất, hệ thống tự tìm lô phù hợp theo FEFO tại chi nhánh nguồn.

2\. **\*\*Duyệt\*\*** (\`PUT /api/PhieuDieuChuyen/{id}/duyet\`) hoặc **\*\*Từ chối\*\*** (\`.../tuchoi\`) — Chủ cửa hàng chi nhánh **\*\*nguồn\*\*** xem xét.

3\. **\*\*Xuất kho\*\*** (\`PUT .../xuatkho\`) — Nhân viên chi nhánh **\*\*nguồn\*\*** xác nhận đã đóng gói/gửi hàng thật, hệ thống trừ kho tại đây.

4\. **\*\*Nhận hàng\*\*** (\`PUT .../nhanhang\`) — Nhân viên chi nhánh **\*\*đích\*\*** xác nhận đã nhận đủ hàng thật, hệ thống cộng kho tại đây, phiếu chuyển sang \`HoanTat\`.

Trạng thái phiếu đi qua: \`ChoDuyet\` → \`DaDuyet\` (hoặc \`TuChoi\`) → \`DangVanChuyen\` → \`HoanTat\`.

**## 🏷️ Cơ chế khuyến mãi (2 loại song song, phục vụ mục đích khác nhau)**

\| | \`KhuyenMai\` | \`Xả kho theo Lô\` |

\|---|---|---|

\| Mục đích | Marketing chung (lễ, Tết, khai trương chi nhánh mới...) | Xử lý hàng tồn kho **\*\*sắp hết hạn\*\*** |

\| Cách áp dụng | Khách tự nhập **\*\*mã code\*\*** lúc thanh toán | **\*\*Tự động\*\*** hiển thị giá đã giảm trên sản phẩm, khách không cần làm gì |

\| Phạm vi | Toàn đơn hàng (giảm trên tổng tiền) | Riêng 1 lô hàng cụ thể của 1 sản phẩm |

\| Ai quản lý | Admin, Chủ cửa hàng | Chủ cửa hàng (thủ công), hệ thống (tự động) |

**\*\*Xả kho hoạt động theo 2 tầng ưu tiên (tính lại mỗi lần truy vấn, không cần tiến trình nền):\*\***

1\. Nếu Chủ cửa hàng đã **\*\*bấm tay\*\*** (\`PUT /api/LoHang/{id}/xakho\`) và còn trong khoảng ngày hiệu lực → dùng đúng mức đó, **\*\*không bị ghi đè\*\***.

2\. Nếu không có ai can thiệp tay → hệ thống **\*\*tự động\*\*** tra bảng \`BacGiamGiaXaKho\` (mặc định: còn ≤90 ngày giảm 15%, ≤30 ngày giảm 30%, ≤7 ngày giảm 50%) và áp đúng mức tương ứng.

\> Lưu ý: khi **\*\*tạo đơn hàng thật\*\*** (\`POST /api/DonHang\`), hệ thống chỉ tôn trọng mức giảm **\*\*thủ công\*\*** đã xác nhận, không áp mức tự động theo bậc — tránh rủi ro giá biến động ngay lúc chốt đơn.

**## 🛒 Giỏ hàng & Đặt hàng (FEFO thật)**

\- **\*\*Giỏ hàng\*\*** (\`/api/GioHang\`): yêu cầu đăng nhập với vai trò \`KhachHang\`. Hệ thống lấy \`KhachHangId\` từ JWT và kiểm tra quyền sở hữu trước khi xem, cập nhật hoặc xóa chi tiết giỏ hàng. Các route cũ có \`{khachHangId}\` vẫn được giữ để tương thích nhưng sẽ trả về \`403 Forbidden\` nếu ID không trùng với khách hàng trong token.

\- **\*\*Đặt hàng\*\*** (\`POST /api/DonHang\`, yêu cầu vai trò \`KhachHang\`): backend lấy \`KhachHangId\` từ JWT, không tin ID khách hàng do client gửi trong body. Với mỗi sản phẩm trong đơn, hệ thống tự động:

  1. Tìm lô có hạn sử dụng **\*\*gần nhất\*\*** mà **\*\*tự nó đủ\*\*** số lượng đặt (không chia nhỏ qua nhiều lô — nếu không lô nào đủ, báo lỗi rõ ràng kèm tổng tồn kho hiện có).

  2. Áp đúng giá đã giảm (nếu lô đó đang xả kho thủ công).

  3. Áp mã khuyến mãi nếu có (kiểm tra hiệu lực, số lượt còn lại, giá trị đơn tối thiểu).

  4. Trừ kho thật ngay tại lô đã chọn, đồng bộ lại tồn kho tổng của sản phẩm.

  5. Toàn bộ 5 bước trên nằm trong 1 luồng lưu duy nhất — nếu lỗi giữa chừng, mọi thay đổi được hủy bỏ đồng thời (không có chuyện "trừ kho rồi nhưng đơn không tạo").

**## 📋 Danh sách API đầy đủ**

**### Xác thực**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| POST | \`/api/Auth/dangky\` | Mở (khách hàng tự đăng ký) |

\| POST | \`/api/Auth/taonhanvien\` | Chỉ Admin |

\| POST | \`/api/Auth/dangnhap\` | Mở |

**### Sản phẩm, Danh mục, Khuyến mãi, Đánh giá**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/SanPham\`, \`/api/SanPham/{id}\` | Công khai |

\| POST/PUT/DELETE | \`/api/SanPham\` | Admin, ChuCuaHang (xóa mềm) |

\| GET | \`/api/DanhMuc\`, \`/api/DanhMuc/{id}\` | Công khai |

\| POST/PUT/DELETE | \`/api/DanhMuc\` | Admin, ChuCuaHang |

\| GET | \`/api/KhuyenMai\` | Admin, NhanVien, ChuCuaHang |

\| POST/PUT/DELETE | \`/api/KhuyenMai\` | Admin, ChuCuaHang |

\| GET | \`/api/DanhGia\`, \`/api/DanhGia/{id}\` | Công khai |

\| POST | \`/api/DanhGia\` | KhachHang (chỉ đơn của mình đã \`HoanThanh\`) |

\| DELETE | \`/api/DanhGia/{id}\` | Admin, ChuCuaHang |

**### Chi nhánh**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/ChiNhanh\`, \`/{id}\` | Admin, NhanVien, ChuCuaHang |

\| POST/PUT/DELETE | \`/api/ChiNhanh\` | Chỉ Admin |

**### Lô hàng**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/LoHang\`, \`.../{id}\`, \`.../sanpham/{id}\`, \`.../saphethan?soNgay=30\` | Xem theo chi nhánh (Admin xem hết) |

\| POST | \`/api/LoHang\` (nhập lô mới) | Admin, ChuCuaHang |

\| PUT | \`.../{id}/xakho\`, \`.../huyxakho\` | Admin, ChuCuaHang |

\| PUT | \`.../{id}/dieuchinhton\` (điều chỉnh vận hành) | Admin, NhanVien |

**### Bậc giảm giá xả kho**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET/POST/PUT/DELETE | \`/api/BacGiamGia\` | Chỉ Admin |

**### Đơn hàng**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| POST | \`/api/DonHang\` (tạo đơn) | KhachHang; ID khách được lấy từ JWT |

\| GET | \`/api/DonHang\`, \`.../{id}\` | Admin, NhanVien, ChuCuaHang (theo chi nhánh) |

\| PUT | \`.../{id}/trangthai\` | Admin, NhanVien, ChuCuaHang (theo chi nhánh) |

**### Giỏ hàng**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/GioHang\` | KhachHang (giỏ của tài khoản đang đăng nhập) |

\| POST | \`/api/GioHang/them\` | KhachHang |

\| PUT/DELETE | \`.../chitiet/{chiTietId}\` | KhachHang (phải sở hữu chi tiết giỏ) |

\| DELETE | \`/api/GioHang/xoahet\` | KhachHang |

\> Các endpoint cũ có \`{khachHangId}\` vẫn hoạt động để tương thích, nhưng chỉ khi ID trên URL trùng với \`KhachHangId\` trong JWT.

**### Phiếu điều chuyển kho**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/PhieuDieuChuyen\`, \`.../{id}\` | Xem theo chi nhánh liên quan |

\| POST | \`/api/PhieuDieuChuyen\` (tạo yêu cầu) | Admin, ChuCuaHang (chi nhánh nhận) |

\| PUT | \`.../{id}/duyet\`, \`.../tuchoi\` | Admin, ChuCuaHang (chi nhánh nguồn) |

\| PUT | \`.../{id}/xuatkho\` | Admin, NhanVien (chi nhánh nguồn) |

\| PUT | \`.../{id}/nhanhang\` | Admin, NhanVien (chi nhánh đích) |

**### Khách hàng**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/KhachHang\`, \`.../{id}\` | Admin, NhanVien, ChuCuaHang |

\| PUT | \`.../{id}\`, \`.../{id}/trangthai\` | Admin, ChuCuaHang |

**### Thống kê**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/ThongKe/tongquan\`, \`.../doanhthu\`, \`.../sanphambanchay\` | Admin: toàn hệ thống; NhanVien/ChuCuaHang: bị ép về chi nhánh mình dù truyền tham số khác |

**### Hình ảnh sản phẩm, Thông báo**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| GET | \`/api/SanPham/{id}/HinhAnhSanPham\` | Công khai |

\| POST/DELETE | \`/api/SanPham/{id}/HinhAnhSanPham\` | Admin, ChuCuaHang |

\| GET/POST | \`/api/ThongBao\` | Admin, NhanVien, ChuCuaHang |

\| GET | \`/api/ThongBao/cuatoi\` | KhachHang (chỉ thông báo của mình) |

\| GET | \`/api/ThongBao/khachhang/{khachHangId}\` | Nội bộ; KhachHang chỉ được dùng đúng ID của mình |

**### Chatbot AI**

\| Method | Endpoint | Quyền |

\|---|---|---|

\| POST | \`/api/Chatbot/chat\` | KhachHang; ID khách được lấy từ JWT |

\| GET | \`/api/Chatbot/lichsu/{khachHangId}\` | KhachHang (chỉ lịch sử của mình) |

\| PUT | \`/api/Chatbot/dongphien/{cuocHoiThoaiId}\` | KhachHang (chỉ phiên của mình) |

\> Chi tiết đầy đủ tham số, mẫu request/response tại Swagger UI sau khi chạy ứng dụng.

**## 📌 Khắc phục sự cố thường gặp (tổng hợp toàn bộ quá trình)**

\- **\*\*\`Could not find any project in ...\`\*\***: Đang đứng sai thư mục khi chạy lệnh \`dotnet\`. Chạy \`cd Backend/TraSayKho.API\` trước.

\- **\*\*\`The server was not found or was not accessible\`\*\***: Sai tên server trong connection string, hoặc dùng nhầm \`/\` thay vì \`\\\`. Kiểm tra lại bằng SSMS, dùng đúng \`\\\\\` trong JSON.

\- **\*\*\`does not contain a definition for 'XxxYyy'\`\*\***: Sai tên \`DbSet\` khi gọi \`\_context.XxxYyy\`. Mở \`Data/TraSayKhoDbContext.cs\`, đối chiếu đúng tên \`DbSet\` tương ứng (EF Core scaffold đôi khi đặt tên hơi khác dự đoán, ví dụ số ít/nhiều).

\- **\*\*\`Cannot implicitly convert type 'X?' to 'X'\`\*\***: Thiếu xử lý giá trị null (thường ở cột \`date\` hoặc computed column). **\*\*Thêm\*\*** \`?? giá\_trị\_mặc\_định\` vào cuối dòng gán.

\- **\*\*\`Operator '??' cannot be applied to operands of type 'X' and 'X'\`\*\***: Property không phải kiểu nullable nhưng lại dùng \`??\`. **\*\*Xóa\*\*** \`?? giá\_trị\_mặc\_định\` đi.

\- **\*\*\`The type or namespace name 'XxxDto' could not be found\`\*\***: File DTO có thể bị dán thiếu 1 phần (copy sót đoạn cuối). Kiểm tra lại đầy đủ tất cả class trong file DTO liên quan, dán lại toàn bộ nếu cần.

\- **\*\*Chatbot báo lỗi "thiếu API key"\*\***: Chưa cấu hình \`user-secrets\` theo Bước 3.

\- **\*\*Chatbot báo lỗi mã 404 khi gọi AI\*\***: Model Gemini đang cấu hình (\`GeminiApi\:Model\`) đã bị Google ngừng hỗ trợ. Đọc nội dung lỗi trả về (Google thường tự gợi ý tên model mới ngay trong thông báo) rồi cập nhật lại giá trị \`Model\`.

\- **\*\*Sau khi đổi database (thêm bảng/cột mới)\*\***: chạy lại đúng script \`.sql\` mới nhất, rồi scaffold lại Model:

\`\`\`bash

  cd Backend/TraSayKho.API

  dotnet ef dbcontext scaffold "Server=TEN\_SERVER;Database=TraSayKhoDB;Trusted\_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context TraSayKhoDbContext --context-dir Data --no-onconfiguring --force

\`\`\`

  Tham số \`--force\` cho phép ghi đè Model cũ — an toàn vì các file trong \`Models/\`, \`Data/\` không được sửa tay trực tiếp.

\- **\*\*\`401 Unauthorized\`\*\***: chưa đăng nhập/chưa Authorize trên Swagger, hoặc token hết hạn (24h) — đăng nhập lại lấy token mới. Cũng có thể do vừa restart server làm Swagger mất trạng thái Authorize đã lưu trước đó — bấm Authorize lại là được.

\- **\*\*\`403 Forbidden\`\*\***: đã đăng nhập đúng nhưng không có vai trò phù hợp, cố thao tác dữ liệu không thuộc chi nhánh, truy cập dữ liệu của khách hàng khác, hoặc đang dùng token cũ chưa có \`KhachHangId\`/\`NhanVienId\`. Hãy đăng nhập lại để lấy token mới trước khi kiểm tra.

\- **\*\*Nhập token bị lặp "Bearer Bearer"\*\***: trong ô Value của Swagger Authorize, chỉ dán đúng chuỗi token, **\*\*không tự gõ thêm chữ "Bearer"\*\*** (Swagger tự thêm sẵn).

\- **\*\*Máy mỗi người có cấu hình SQL Server khác nhau\*\*** (SQLEXPRESS vs MSSQLSERVER, tên instance khác nhau): luôn tự kiểm tra và sửa lại \`appsettings.json\` theo máy mình, không copy nguyên giá trị của người khác.
