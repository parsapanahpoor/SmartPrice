# ⚡ Quick Start - اجرای سریع SmartPrice

## 🎯 گام‌های اجرا (5 دقیقه)

### 1️⃣ راه‌اندازی Database & Redis
```bash
docker-compose -f docker-compose.dev.yml up -d
```

### 2️⃣ اجرای Migration
```bash
cd src\SmartPrice.API
dotnet ef database update --project ..\SmartPrice.Infrastructure
```

### 3️⃣ اجرای API
```bash
dotnet run
```

### 4️⃣ باز کردن Swagger
```
http://localhost:5000
```

### 5️⃣ Login
```json
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

### 6️⃣ Authorize در Swagger
کلیک روی Authorize و وارد کنید:
```
Bearer {accessToken}
```

### 7️⃣ تست Dashboard
```
GET /api/admin/dashboard
```

## ✅ موفق!
