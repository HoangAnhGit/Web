Here's the improved `README.md` file, incorporating the new content while maintaining the existing structure and information:

# Quản lý Cho thuê Phòng trọ (Room Rental Management System)			

## Giới thiệu
Hệ thống web hỗ trợ quản lý hoạt động cho thuê phòng trọ với 3 vai trò chính:
- **Admin**: Quản lý người dùng toàn hệ thống.
- **Landlord (Chủ nhà)**: Quản lý phòng và duyệt hợp đồng thuê.
- **Tenant (Người thuê)**: Tìm kiếm phòng, tạo hợp đồng và thanh toán.

Dự án được xây dựng bằng **ASP.NET Core 8**, dùng **Entity Framework Core** với **PostgreSQL**, giao diện **Razor Views + Bootstrap 5**, kết hợp **AJAX** cho trải nghiệm tìm kiếm và thao tác nhanh.

## Mục tiêu dự án
- Quản lý phòng trọ theo mô hình CRUD.
- Xác thực người dùng và phân quyền theo vai trò.
- Quản lý vòng đời hợp đồng thuê (Pending/Active/Terminated).
- Quản lý thanh toán theo hợp đồng.
- Tìm kiếm/lọc phòng nâng cao theo nhiều tiêu chí.
- Tối ưu hiệu năng bằng **In-Memory Cache**.

## Công nghệ sử dụng
- **Backend**: ASP.NET Core 8 (MVC + Razor Pages support)
- **ORM**: Entity Framework Core 8
- **Database**: PostgreSQL + Npgsql
- **Frontend**: Razor, Bootstrap 5, jQuery
- **Auth**: Session-based Authentication
- **Security**: CSRF Token, Password Hashing (PBKDF2 SHA256)
- **Caching**: `IMemoryCache`

## Kiến trúc tổng quan
Dự án áp dụng kiến trúc phân tầng:
- `Controllers`: xử lý request/response và điều hướng.
- `Services`: business logic (ví dụ cache phòng, xác thực).
- `Repositories`: truy cập dữ liệu qua EF Core.
- `Data/ApplicationDbContext`: cấu hình mô hình và quan hệ DB.
- `Views`: giao diện Razor theo từng module.

Luồng tổng quát:
1. User gửi request từ UI.
2. `Controller` xử lý và gọi `Service`/`Repository`.
3. `Repository` tương tác DB qua `ApplicationDbContext`.
4. Kết quả trả về `View` hoặc JSON (AJAX).

## Cấu trúc thư mục
QuanLyChoThuePhongTro/
├── Controllers/
│   ├── AuthController.cs
│   ├── HomeController.cs
│   ├── RoomController.cs
│   ├── RentalContractController.cs
│   ├── PaymentController.cs
│   └── UserController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── User.cs
│   ├── Room.cs
│   ├── RoomFilter.cs
│   ├── RentalContract.cs
│   └── Payment.cs
├── Repositories/
│   ├── UserRepository.cs
│   ├── RoomRepository.cs
│   ├── RentalContractRepository.cs
│   └── PaymentRepository.cs
├── Services/
│   ├── AuthenticationService.cs
│   ├── IRoomService.cs
│   └── RoomService.cs
├── Views/
│   ├── Auth/
│   ├── Home/
│   ├── Room/
│   ├── RentalContract/
│   ├── Payment/
│   ├── User/
│   └── Shared/
├── Migrations/
├── wwwroot/
├── Program.cs
├── appsettings.json
└── init_database.sql

## Thiết kế dữ liệu (Entity chính)
### `User`
- `Id`, `Username`, `Email`, `PasswordHash`, `FullName`, `PhoneNumber`, `Address`
- `Role`: `Admin | Landlord | Tenant`
- `IsActive`, `CreatedDate`, `UpdatedDate`

### `Room`
- `Title`, `Description`, `Price`, `Location`, `District`, `Ward`, `Area`
- `Bedrooms`, `Bathrooms`, `HasKitchen`, `HasWiFi`, `HasAirConditioner`, `HasWashing`
- `Status`: `Available | Rented | Maintenance`
- `OwnerId`, `ImageUrls`, `CreatedDate`, `UpdatedDate`

### `RentalContract`
- `RoomId`, `TenantId`, `LandlordId`
- `MonthlyPrice`, `Deposit`
- `StartDate`, `EndDate`
- `Status`: `Pending | Active | Expired | Terminated`
- `TermsAndConditions`, `LastPaymentDate`, `RemainingDeposit`

### `Payment`
- `ContractId`, `Amount`, `PaymentDate`, `PaymentMethod`
- `Status`: `Pending | Completed | Failed`
- `Notes`, `CreatedDate`

## Chức năng chính theo module
### 1) Xác thực (`Auth`)
- Đăng ký tài khoản.
- Đăng nhập/đăng xuất.
- Lưu thông tin phiên đăng nhập qua Session (`UserId`, `Username`, `Role`).
- Chặn đăng ký role `Admin` từ UI công khai.

### 2) Quản lý phòng (`Room`)
- CRUD phòng trọ.
- Upload nhiều ảnh khi tạo/sửa phòng.
- Phân quyền: chỉ chủ phòng mới sửa/xóa phòng đó.
- Tìm kiếm nâng cao bằng AJAX + partial view (`_RoomList`).

### 3) Quản lý hợp đồng (`RentalContract`)
- Tenant tạo hợp đồng từ phòng đang quan tâm.
- Landlord duyệt (`Approve`) hoặc từ chối (`Reject`) hợp đồng.
- Khi duyệt hợp đồng: phòng tự động chuyển trạng thái `Rented`.
- Hỗ trợ cả thao tác thường và AJAX.

### 4) Quản lý thanh toán (`Payment`)
- Tenant tạo thanh toán cho hợp đồng đang `Active`.
- Thanh toán nhanh (`QuickPay`).
- Lưu lịch sử thanh toán theo hợp đồng.
- Cập nhật `LastPaymentDate` cho hợp đồng sau mỗi giao dịch.

### 5) Quản lý người dùng (`User`)
- Admin xem/sửa/xóa người dùng.
- Admin không thể tự xóa tài khoản của chính mình.
- Người dùng cập nhật hồ sơ cá nhân tại `Profile`.

### 6) Dashboard (`Home/Dashboard`)
- Hiển thị tổng số phòng, tổng hợp đồng, hợp đồng hiệu lực.
- Tính doanh thu dựa trên các giao dịch `Completed`.

## Bảo mật và hiệu năng
### Bảo mật
- Hash mật khẩu bằng **PBKDF2 SHA256** (`Rfc2898DeriveBytes`).
- Kiểm tra quyền theo role và ownership tại các action quan trọng.
- Dùng `[ValidateAntiForgeryToken]` để chống CSRF với các POST request.
- Session timeout 30 phút.

### Hiệu năng
- Cache danh sách phòng và phòng chi tiết bằng `IMemoryCache`.
- Tự động xóa cache sau khi thêm/sửa/xóa phòng.
- Truy vấn bất đồng bộ (`async/await`) xuyên suốt repository/service.

## Yêu cầu môi trường
- .NET SDK 8.0+
- PostgreSQL 12+
- Visual Studio 2022 / VS Code
- `dotnet-ef` tool

## Hướng dẫn cài đặt nhanh
### 1) Clone mã nguồn
git clone <repository-url>
cd <project-folder>

### 2) Cấu hình database
- Tạo DB thủ công hoặc chạy script:
psql -U postgres -f init_database.sql

- Cập nhật chuỗi kết nối trong `appsettings.json`:
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=5432;Database=quan_ly_cho_thue_phong_tro;User Id=postgres;Password=<YOUR_PASSWORD>"
}

### 3) Restore + migrate
dotnet restore
dotnet ef database update

### 4) Chạy ứng dụng
dotnet run

> Ứng dụng tự động chạy migration khi startup và seed tài khoản admin nếu chưa tồn tại.

## Tài khoản mặc định (seed)
Khi chạy lần đầu, hệ thống tự tạo tài khoản:
- Username: `admin`
- Password: `admin123`
- Role: `Admin`

> Khuyến nghị đổi mật khẩu ngay sau khi đăng nhập lần đầu.

## API/Route thao tác nổi bật
- `GET /Room` — danh sách phòng.
- `POST /Room/Search` — tìm kiếm phòng (trả partial HTML).
- `POST /RentalContract/Approve/{id}` — duyệt hợp đồng.
- `POST /RentalContract/Reject/{id}` — từ chối hợp đồng.
- `POST /Payment/QuickPay/{contractId}` — thanh toán nhanh.

## Kịch bản nghiệp vụ mẫu
### Kịch bản 1: Thuê phòng
1. Tenant đăng nhập.
2. Tenant tìm phòng phù hợp.
3. Tenant tạo hợp đồng (trạng thái `Pending`).
4. Landlord duyệt hợp đồng -> `Active`.
5. Tenant thanh toán kỳ đầu.

### Kịch bản 2: Chủ nhà quản lý phòng
1. Landlord đăng nhập.
2. Tạo phòng mới + upload ảnh.
3. Chỉnh sửa thông tin/giá/tình trạng.
4. Xóa phòng không còn kinh doanh.

## Kiểm thử khuyến nghị
- Đăng nhập với từng role và xác minh quyền truy cập.
- Kiểm tra tạo/sửa/xóa phòng bởi đúng chủ sở hữu.
- Kiểm tra luồng hợp đồng `Pending -> Active/Terminated`.
- Kiểm tra thanh toán chỉ cho hợp đồng `Active`.
- Kiểm tra tìm kiếm với nhiều bộ lọc đồng thời.

## Troubleshooting
### Không kết nối được PostgreSQL
- Kiểm tra service PostgreSQL đang chạy.
- Kiểm tra `Port`, `User Id`, `Password` trong connection string.

### Lỗi migrate
dotnet ef migrations add InitialCreate
dotnet ef database update

### Lỗi thiếu package
dotnet restore

## Định hướng phát triển
- Tích hợp bản đồ và định vị.
- Tích hợp cổng thanh toán thực tế.
- Tách ảnh phòng thành bảng riêng thay vì chuỗi CSV.
- Thêm thông báo email/sms.
- Viết unit test/integration test.
- Docker hóa và CI/CD.

## License
Dự án phục vụ mục đích học tập và nghiên cứu.

This version maintains the original structure while ensuring clarity and coherence throughout the document. Each section is clearly defined, and the content flows logically from one topic to the next.