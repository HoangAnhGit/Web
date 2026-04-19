-- Quản Lý Cho Thuê Phòng Trọ - Database Initialization Script
-- Chạy script này để cài đặt database và user
-- Sử dụng: psql -U postgres -f init_database.sql

-- 1. Tạo database
CREATE DATABASE quan_ly_cho_thue_phong_tro
    WITH
    ENCODING 'UTF8'
    LC_COLLATE 'C'
    LC_CTYPE 'C'
    TEMPLATE template0;

-- 2. Tạo user mới (tùy chọn - nếu muốn dùng user riêng)
-- Uncomment dòng dưới nếu muốn tạo user mới
-- CREATE USER quan_ly_user WITH PASSWORD 'quan_ly_password_123';

-- 3. Cấp quyền cho user postgres
GRANT ALL PRIVILEGES ON DATABASE quan_ly_cho_thue_phong_tro TO postgres;

-- 4. Cấp quyền cho user quan_ly_user (nếu tạo)
-- GRANT ALL PRIVILEGES ON DATABASE quan_ly_cho_thue_phong_tro TO quan_ly_user;

-- 5. Kết nối database
\c quan_ly_cho_thue_phong_tro

-- 6. Cấp quyền schema
GRANT ALL PRIVILEGES ON SCHEMA public TO postgres;
-- GRANT ALL PRIVILEGES ON SCHEMA public TO quan_ly_user;

-- 7. Tạo extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- 8. Kiểm tra database vừa tạo
SELECT datname FROM pg_database WHERE datname = 'quan_ly_cho_thue_phong_tro';

\echo 'Database đã được tạo thành công!'
\echo 'Sử dụng connection string sau:'
\echo 'Server=localhost;Port=5432;Database=quan_ly_cho_thue_phong_tro;User Id=postgres;Password=YOUR_PASSWORD;'
