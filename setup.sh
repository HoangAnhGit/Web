#!/bin/bash

# Quản Lý Cho Thuê Phòng Trọ - Quick Start Script

echo "================================"
echo " Quản Lý Cho Thuê Phòng Trọ"
echo " Quick Start Script"
echo "================================"
echo ""

# Kiểm tra .NET SDK
echo "[1/5] Kiểm tra .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo "LỖI: .NET SDK chưa được cài đặt!"
    echo "Tải từ: https://dotnet.microsoft.com/download"
    exit 1
fi
echo "✓ .NET SDK được tìm thấy: $(dotnet --version)"

# Kiểm tra Git
echo ""
echo "[2/5] Kiểm tra Git..."
if ! command -v git &> /dev/null; then
    echo "CẢNH BÁO: Git chưa được cài đặt"
else
    echo "✓ Git được tìm thấy: $(git --version)"
fi

# Khôi phục packages
echo ""
echo "[3/5] Khôi phục NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "LỖI: Không thể khôi phục packages!"
    exit 1
fi
echo "✓ Packages khôi phục thành công"

# Kiểm tra PostgreSQL
echo ""
echo "[4/5] Kiểm tra PostgreSQL..."
if ! command -v psql &> /dev/null; then
    echo "CẢNH BÁO: PostgreSQL CLI không tìm thấy"
else
    psql -U postgres -h localhost -w -c "SELECT version();" &> /dev/null
    if [ $? -eq 0 ]; then
        echo "✓ PostgreSQL kết nối được"
    else
        echo "CẢNH BÁO: PostgreSQL không phản hồi"
        echo "Đảm bảo PostgreSQL đang chạy"
        read -p "Bạn có muốn tiếp tục không? (y/n) " -n 1 -r
        echo ""
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            exit 1
        fi
    fi
fi

# Kill existing process if running
pkill -f QuanLyChoThuePhongTro > /dev/null 2>&1

# Build project
echo ""
echo "[5/5] Build project..."
dotnet build
if [ $? -ne 0 ]; then
    echo "LỖI: Build project thất bại!"
    exit 1
fi
echo "✓ Build project thành công"

echo ""
echo "================================"
echo " Cài đặt hoàn tất!"
echo "================================"
echo ""
echo "Chạy ứng dụng với lệnh:"
echo "  dotnet run"
echo "hoặc (với auto-reload)"
echo "  dotnet watch"
echo ""
echo "Truy cập: https://localhost:7000"
echo ""
