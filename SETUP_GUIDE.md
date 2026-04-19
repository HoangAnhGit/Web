# Hướng Dẫn Chi Tiết Cài Đặt Hệ Thống

## Mục Lục

1. [Cài đặt .NET 8.0 SDK](#cài-đặt-net-80-sdk)
2. [Cài đặt PostgreSQL](#cài-đặt-postgresql)
3. [Cài đặt VS Code & Extensions](#cài-đặt-vs-code--extensions)
4. [Clone & Cấu hình Dự án](#clone--cấu-hình-dự-án)
5. [Thiết lập Database](#thiết-lập-database)
6. [Chạy Ứng dụng](#chạy-ứng-dụng)

---

## Cài đặt .NET 8.0 SDK

### Windows

1. Truy cập https://dotnet.microsoft.com/download
2. Chọn **.NET 8.0 SDK**
3. Tải **Windows x64** (hoặc x86 nếu dùng 32-bit)
4. Chạy file installer (.exe)
5. Follow Installation Wizard:
   - Chọn **Install**
   - Chọn vị trí cài đặt (mặc định: `C:\Program Files\dotnet`)
   - Finish

**Xác minh cài đặt:**
```bash
# Mở Command Prompt (Win + R → cmd)
dotnet --version

# Output: 8.0.x
```

### macOS

```bash
# Cách 1: Dùng Homebrew (khuyến nghị)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
brew install dotnet

# Cách 2: Tải installer
# https://dotnet.microsoft.com/download/dotnet/8.0
# Chọn macOS, rồi chọn Installer (.dmg)
# Chạy file .dmg

# Xác minh
dotnet --version
```

### Linux (Ubuntu/Debian)

```bash
# Cập nhật package manager
sudo apt update
sudo apt upgrade -y

# Cài .NET 8.0 SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0 --install-dir ~/dotnet

# Thêm vào PATH
echo 'export PATH="$HOME/dotnet:$PATH"' >> ~/.bashrc
source ~/.bashrc

# Xác minh
dotnet --version
```

---

## Cài đặt PostgreSQL

### Windows

1. Tải từ: https://www.postgresql.org/download/windows/
2. Chọn installer mới nhất (PostgreSQL 15+)
3. Chạy installer:
   - **Installation Directory**: Để default (C:\Program Files\PostgreSQL\15)
   - **Port**: 5432 (default)
   - **Superuser**: postgres
   - **Password**: Chọn password mạnh, ví dụ: `postgres123`
   - **Locale**: Default
4. Finish

**Xác minh:**
```bash
# Mở cmd
psql -U postgres -h localhost

# Nhập password khi được hỏi
# Nếu thành công sẽ thấy: postgres=#
\q  # Thoát
```

### macOS

```bash
# Cách 1: Homebrew (khuyến nghị)
brew install postgresql@15
brew services start postgresql@15

# Kiểm tra
psql -U postgres

# Cách 2: Tải app
# https://www.postgresql.org/download/macosx/
# Chọn PostgreSQL installer
# Sau khi cài, khởi động server từ menu bar
```

### Linux (Ubuntu/Debian)

```bash
# Cài PostgreSQL
sudo apt update
sudo apt install postgresql postgresql-contrib

# Xác minh service đang chạy
sudo systemctl status postgresql

# Kết nối database
sudo -u postgres psql

# Thoát
\q
```

---

## Cài đặt VS Code & Extensions

### 1. Cài đặt VS Code

- Tải từ: https://code.visualstudio.com/
- Chạy installer theo hướng dẫn

### 2. Cài đặt Extensions

Mở VS Code → Extensions (Ctrl+Shift+X):

| Extension | Publisher | Tác vụ |
|-----------|-----------|--------|
| **C# Dev Kit** | Microsoft | Hỗ trợ C# và .NET development |
| **Entity Framework Core Power Tools** | Eric Jacobson | Công cụ hỗ trợ EF Core |
| **PostgreSQL Explorer** | chutibox | Quản lý database PostgreSQL |
| **Thunder Client** | Ranga Vadhineni | API testing (thay cho Postman) |

**Cài đặt từ Command Palette:**
```bash
Ctrl+Shift+P → "Extensions: Install Extensions"
Tìm từng extension và click Install
```

---

## Clone & Cấu hình Dự án

### 1. Clone Dự án

```bash
# Mở Terminal/PowerShell

# Di chuyển đến thư mục muốn lưu
cd D:\Projects
# hoặc
cd ~/Projects

# Clone repository
git clone [REPOSITORY_URL]

# Vào thư mục project
cd QuanLyChoThuePhongTro

# Mở trong VS Code
code .
```

### 2. Khôi phục Dependencies

```bash
# Cài đặt NuGet packages
dotnet restore

# Kiểm tra
dotnet build
```

---

## Thiết lập Database

### Phương pháp 1: Dùng psql (Command Line)

```bash
# Kết nối PostgreSQL
psql -U postgres -h localhost

# Nhập password postgres

# Tạo database
CREATE DATABASE quan_ly_cho_thue_phong_tro
  ENCODING 'UTF8'
  TEMPLATE template0;

# Tạo user (tùy chọn)
CREATE USER quan_ly_user WITH PASSWORD 'password123';
GRANT ALL PRIVILEGES ON DATABASE quan_ly_cho_thue_phong_tro TO quan_ly_user;

# Kiểm tra
\l

# Thoát
\q
```

### Phương pháp 2: Dùng pgAdmin (GUI)

1. Mở pgAdmin (tự động cài với PostgreSQL trên Windows)
2. URL: http://localhost:5050
3. Đăng nhập (password được tạo lúc cài)
4. Right-click **Databases** → **Create** → **Database**
   - Name: `quan_ly_cho_thue_phong_tro`
   - Owner: `postgres`
   - Click **Save**

### Cấu hình Connection String

Mở file `appsettings.json`:

```json
{
  "Logging": { ... },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=quan_ly_cho_thue_phong_tro;User Id=postgres;Password=postgres123;"
  }
}
```

**Lưu ý:**
- Thay `postgres123` bằng password PostgreSQL của bạn
- Nếu dùng user khác, thay `postgres` bằng username
- Database name phải khớp với database đã tạo

---

## Chạy Ứng dụng

### 1. Tạo Migration Ban Đầu

```bash
# Cài đặt EF Core CLI (nếu chưa có)
dotnet tool install --global dotnet-ef

# Hoặc update nếu đã có
dotnet tool update --global dotnet-ef

# Tạo migration
dotnet ef migrations add InitialCreate

# Kiểm tra file migration được tạo
# Nằm trong folder Migrations/
```

### 2. Áp dụng Database Migration

```bash
# Tạo tables trong database
dotnet ef database update

# Xác minh trong pgAdmin:
# QuanLyChoThuePhongTro → Schemas → public → Tables
```

### 3. Chạy Application

```bash
# Cách 1: Chạy bình thường
dotnet run

# Cách 2: Watch mode (tự reload khi có thay đổi)
dotnet watch

# Output sẽ hiển thị URLs:
# https://localhost:7xxx
# http://localhost:5xxx
```

### 4. Truy cập Application

- Mở browser
- Truy cập: `https://localhost:7xxx` (theo output)
- Click "Accept" nếu có cảnh báo SSL

---

## Tài Khoản Test

### Đăng Ký Tài Khoản Đầu Tiên

1. Click **Đăng ký**
2. Điền thông tin:
   - **Tên đăng nhập**: `lanhuong`
   - **Email**: `lanhuong@example.com`
   - **Họ tên**: `Lân Hương`
   - **Mật khẩu**: `Password123`
   - **Vai trò**: Chọn "Chủ nhà" hoặc "Người thuê"
3. Click **Đăng ký**

### Đăng Nhập

1. Click **Đăng nhập**
2. Nhập:
   - **Tên đăng nhập**: `lanhuong`
   - **Mật khẩu**: `Password123`
3. Click **Đăng nhập**

---

## Các Lệnh Hữu Ích

```bash
# Build project
dotnet build

# Clean build
dotnet clean && dotnet build

# Run tests (nếu có)
dotnet test

# Publish
dotnet publish -c Release

# Xem migrations hiện tại
dotnet ef migrations list

# Rollback migration
dotnet ef database update <PreviousMigration>

# Xóa migration cuối cùng
dotnet ef migrations remove

# Reset database (xóa tất cả data)
dotnet ef database drop --force
dotnet ef database update
```

---

## Troubleshooting

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-----------|----------|
| `Failed to connect to localhost:5432` | PostgreSQL không chạy | `sudo systemctl start postgresql` |
| `Database ... doesn't exist` | Database chưa tạo | Tạo database qua psql hoặc pgAdmin |
| `password authentication failed` | Sai password | Kiểm tra `appsettings.json` |
| `The .NET SDK version is not supported` | .NET version sai | Cài .NET 8.0 SDK |
| `Unable to resolve service` | Missing dependency injection | Kiểm tra `Program.cs` |

---

## Tài Liệu Tham Khảo

- [.NET 8.0 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)

---

**Phiên bản**: 1.0  
**Cập nhật**: Tháng 1 năm 2026  
**Hỗ trợ**: Liên hệ giảng viên
