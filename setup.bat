@echo off
REM Quán Lý Cho Thuê Phòng Trọ - Quick Start Script
REM Script này giúp thiết lập và chạy dự án nhanh chóng

setlocal enabledelayedexpansion

echo ================================
echo  Quán Lý Cho Thuê Phòng Trọ
echo  Quick Start Script
echo ================================
echo.

REM Kiểm tra .NET SDK
echo [1/5] Kiểm tra .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo LỖI: .NET SDK chưa được cài đặt!
    echo Tải từ: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✓ .NET SDK được tìm thấy

REM Kiểm tra Git
echo.
echo [2/5] Kiểm tra Git...
git --version >nul 2>&1
if errorlevel 1 (
    echo CẢNH BÁO: Git chưa được cài đặt
) else (
    echo ✓ Git được tìm thấy
)

REM Khôi phục packages
echo.
echo [3/5] Khôi phục NuGet packages...
dotnet restore
if errorlevel 1 (
    echo LỖI: Không thể khôi phục packages!
    pause
    exit /b 1
)
echo ✓ Packages khôi phục thành công

REM Kiểm tra PostgreSQL
echo.
echo [4/5] Kiểm tra PostgreSQL...
psql -U postgres -h localhost -w -c "SELECT version();" >nul 2>&1
if errorlevel 1 (
    echo CẢNH BÁO: PostgreSQL không phản hồi
    echo Đảm bảo PostgreSQL đang chạy và connection string đúng
    echo Xem SETUP_GUIDE.md để tìm hiểu thêm
    echo.
    pause
) else (
    echo ✓ PostgreSQL kết nối được
)

REM Kill existing process if running to prevent file lock errors
taskkill /F /IM QuanLyChoThuePhongTro.exe >nul 2>&1

REM Build project
echo.
echo [5/5] Build project...
dotnet build
if errorlevel 1 (
    echo LỖI: Build project thất bại!
    pause
    exit /b 1
)
echo ✓ Build project thành công

echo.
echo ================================
echo  Cài đặt hoàn tất!
echo ================================
echo.
echo Nhập lệnh để chạy:
echo   dotnet run
echo hoặc
echo   dotnet watch (tự động reload)
echo.
echo Truy cập: https://localhost:7000
echo.
pause
