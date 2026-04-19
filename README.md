# Quản lý Cho thuê Phòng trọ (Room Rental Management System)

## Mục tiêu đề tài

- **CRUD Operations**: Quản lý thông tin phòng trọ (Create, Read, Update, Delete)
- **Dynamic Data Display**: Hiển thị dữ liệu động qua Ajax hoặc Fetch API
- **Security**: Xác thực người dùng (Authentication), phân quyền (Authorization), Session, Cookie
- **Caching**: Áp dụng kỹ thuật Caching để tăng hiệu năng
- **User Interface**: Giao diện thân thiện, có báo cáo thống kê

## Yêu cầu nghiệp vụ

1. **CRUD Phòng trọ**: Quản lý thông tin phòng trọ (địa chỉ, giá, tiện ích, v.v.)
2. **Tìm kiếm**: Tìm kiếm phòng theo vị trí, giá cả
3. **Đăng ký thuê online**: Người dùng có thể đăng ký thuê phòng trực tuyến
4. **Quản lý hợp đồng**: Quản lý hợp đồng thuê phòng

## Công nghệ sử dụng

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Frontend**: HTML, CSS, Bootstrap 5, JavaScript
- **Authentication**: Session-based
- **Caching**: In-Memory Cache

## Cấu trúc dự án

```
QuanLyChoThuePhongTro/
├── Models/                  # Các lớp dữ liệu
│   ├── User.cs
│   ├── Room.cs
│   ├── RentalContract.cs
│   └── Payment.cs
├── Controllers/             # Các controller
│   ├── AuthController.cs
│   ├── RoomController.cs
│   ├── RentalContractController.cs
│   ├── UserController.cs
│   └── PaymentController.cs
├── Services/               # Business logic
│   ├── RoomService.cs
│   └── AuthenticationService.cs
├── Repositories/           # Data access
│   ├── RoomRepository.cs
│   ├── RentalContractRepository.cs
│   ├── UserRepository.cs
│   └── PaymentRepository.cs
├── Data/                   # Database context
│   └── ApplicationDbContext.cs
├── Views/                  # Razor views
│   ├── Home/
│   ├── Room/
│   ├── Auth/
│   ├── RentalContract/
│   ├── User/
│   ├── Payment/
│   └── Shared/
├── wwwroot/               # Static files
│   ├── css/
│   └── js/
├── Program.cs            # Entry point
├── appsettings.json      # Configuration
└── QuanLyChoThuePhongTro.csproj
```

## Hướng dẫn cài đặt

### Yêu cầu hệ thống

- .NET 8.0 SDK trở lên
- PostgreSQL 12 trở lên
- Visual Studio 2022 hoặc VS Code
- Git

### Cài đặt các công nghệ liên quan

#### 1. Cài đặt .NET 8.0 SDK

**Windows:**
- Tải từ: https://dotnet.microsoft.com/download
- Chọn `.NET 8.0 SDK` (Windows x64 hoặc x86)
- Chạy installer và follow hướng dẫn
- Kiểm tra: `dotnet --version`

**macOS:**
```bash
# Sử dụng Homebrew
brew install dotnet

# Hoặc tải từ: https://dotnet.microsoft.com/download
```

**Linux (Ubuntu/Debian):**
```bash
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0
```

#### 2. Cài đặt PostgreSQL

**Windows:**
- Tải từ: https://www.postgresql.org/download/windows/
- Chọn phiên bản mới nhất (15 hoặc cao hơn)
- Chạy installer
- **Nhớ password cho user `postgres`**
- Chọn port mặc định: `5432`
- Chọn cài đặt `pgAdmin` (GUI tool)

**macOS:**
```bash
# Sử dụng Homebrew
brew install postgresql

# Khởi động PostgreSQL
brew services start postgresql

# Hoặc tải từ: https://www.postgresql.org/download/macosx/
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib

# Khởi động service
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

#### 3. Cài đặt Visual Studio Code (tùy chọn)

- Tải từ: https://code.visualstudio.com/
- Extensions cần thiết:
  - **C# Dev Kit** (Microsoft)
  - **Entity Framework Core Power Tools** (Eric Jacobson)
  - **PostgreSQL Explorer** (chutibox)

#### 4. Cài đặt pgAdmin (GUI cho PostgreSQL)

- Tải từ: https://www.pgadmin.org/download/
- Hoặc cài qua Homebrew: `brew install pgadmin4`
- Truy cập: http://localhost:5050

### Các bước cài đặt dự án

#### 1. Clone hoặc tải dự án
```bash
# Clone từ repository
git clone [repository-url]
cd QuanLyChoThuePhongTro

# Hoặc giải nén nếu tải file zip
cd QuanLyChoThuePhongTro
```

#### 2. Cấu hình PostgreSQL

**Tạo database và user (dùng psql):**

Mở Command Prompt/Terminal và kết nối:
```bash
psql -U postgres
```

Nhập password đã tạo khi cài PostgreSQL.

Sau đó chạy các lệnh SQL:
```sql
-- Tạo user mới (tùy chọn)
CREATE USER quan_ly_user WITH PASSWORD 'password123';

-- Tạo database
CREATE DATABASE quan_ly_cho_thue_phong_tro OWNER postgres;

-- Hoặc nếu dùng user vừa tạo
CREATE DATABASE quan_ly_cho_thue_phong_tro OWNER quan_ly_user;

-- Cấp quyền
GRANT ALL PRIVILEGES ON DATABASE quan_ly_cho_thue_phong_tro TO postgres;

-- Kết nối database
\c quan_ly_cho_thue_phong_tro

-- Kiểm tra
\dt
```

**Cách khác (dùng pgAdmin):**
1. Mở pgAdmin
2. Kết nối đến Server `localhost`
3. Chuột phải `Databases` → `Create` → `Database`
4. Tên: `quan_ly_cho_thue_phong_tro`
5. Click `Save`

#### 3. Cấu hình connection string

Mở file [appsettings.json](appsettings.json) và cập nhật:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=5432;Database=quan_ly_cho_thue_phong_tro;User Id=postgres;Password=YOUR_PASSWORD;"
}
```

Thay `YOUR_PASSWORD` bằng password của user `postgres` hoặc user tương ứng.

#### 4. Cài đặt dependencies

```bash
# Restore NuGet packages
dotnet restore
```

#### 5. Tạo migrations và database

```bash
# Cài đặt Entity Framework CLI (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration ban đầu
dotnet ef migrations add InitialCreate

# Apply migrations tới database
dotnet ef database update
```

#### 6. Chạy dự án

```bash
# Chạy application
dotnet run

# Hoặc sử dụng watch mode (tự động reload)
dotnet watch
```

Ứng dụng sẽ chạy tại: `https://localhost:7xxx` (kiểm tra output để biết port)

#### 7. Truy cập ứng dụng

- **URL**: https://localhost:5001 hoặc http://localhost:5000
- **Đăng ký tài khoản** hoặc sử dụng tài khoản test

### Troubleshooting

**Lỗi: "Failed to connect to localhost:5432"**
- Kiểm tra PostgreSQL đang chạy: `pg_isready`
- Restart PostgreSQL service
- Kiểm tra connection string trong `appsettings.json`

**Lỗi: "Database quan_ly_cho_thue_phong_tro doesn't exist"**
```bash
# Tạo database qua command line
createdb -U postgres quan_ly_cho_thue_phong_tro

# Hoặc dùng pgAdmin
```

**Lỗi: "password authentication failed"**
- Kiểm tra password trong `appsettings.json`
- Reset password PostgreSQL (Windows):
  ```bash
  pg_dump -U postgres -d postgres > backup.sql
  ```

**Lỗi: "A project file was not found in"**
- Chắc chắn bạn đang trong thư mục chứa file `.csproj`
- Kiểm tra tên thư mục không có khoảng trắng đặc biệt

## Các vai trò người dùng

1. **Admin**: Quản lý toàn bộ hệ thống
2. **Landlord** (Chủ nhà): Đăng ký phòng, quản lý hợp đồng của mình
3. **Tenant** (Người thuê): Tìm kiếm phòng, đăng ký thuê, quản lý hợp đồng của mình

## Các tính năng chính

### Quản lý Phòng
- Tạo phòng mới (chỉ Landlord)
- Xem danh sách phòng
- Cập nhật thông tin phòng
- Xóa phòng
- Tìm kiếm phòng theo vị trí, giá cả
- Caching dữ liệu phòng

### Quản lý Người dùng (Chỉ Admin)
- Xem danh sách toàn bộ người dùng
- Chỉnh sửa thông tin người dùng (Vai trò, trạng thái)
- Xóa người dùng (có xác nhận)
- Phân quyền người dùng (Admin, Landlord, Tenant)

### Xác thực & Phân quyền
- Đăng ký tài khoản
- Đăng nhập
- Đăng xuất
- Session management
- Role-based access control

### Quản lý Hợp đồng
- Tạo hợp đồng thuê
- Xem danh sách hợp đồng
- Cập nhật trạng thái hợp đồng
- Xóa hợp đồng (chỉ Landlord)

### Quản lý Thanh toán
- Tạo thanh toán cho hợp đồng (Tenant)
- Xem lịch sử thanh toán của hợp đồng
- Thanh toán nhanh (Quick Pay)
- Xem chi tiết biên lai thanh toán

### Tìm kiếm & Lọc
- Tìm kiếm theo vị trí
- Lọc theo khoảng giá
- Hiển thị kết quả động (Ajax)

## Bảo mật

- **Password Hashing**: Sử dụng PBKDF2 với SHA256
- **Session Management**: Timeout 30 phút
- **Authorization**: Kiểm tra quyền truy cập dựa trên vai trò
- **CSRF Protection**: Token validation trong form

## Hiệu năng

- **In-Memory Cache**: Cache danh sách phòng với TTL 30 phút
- **Lazy Loading**: Entity relationships được load khi cần
- **Async/Await**: Tất cả database operations đều async

## Hướng phát triển

- [x] Thêm chức năng thanh toán online
- [ ] Tích hợp bản đồ (Google Maps)
- [ ] Upload hình ảnh phòng
- [ ] Hệ thống đánh giá & bình luận
- [ ] Email notifications
- [ ] Mobile app
- [ ] Analytics dashboard



## Ngày tạo

Tháng 1 năm 2026

---

**Note**: Đây là dự án hoc tập cho môn học Phát triển Ứng dụng Web nâng cao
