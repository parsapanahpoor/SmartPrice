# 🚀 راهنمای اجرا و تست SmartPrice

## پیش‌نیازها

- ✅ .NET 7 SDK
- ✅ Docker Desktop (برای PostgreSQL و Redis)
- ✅ یک HTTP Client (Postman یا استفاده از Swagger)

---

## گام 1: راه‌اندازی Database و Redis

### روش 1: استفاده از Docker (پیشنهادی)

```bash
# در دایرکتوری root پروژه
docker-compose -f docker-compose.dev.yml up -d
```

این دستور موارد زیر را راه‌اندازی می‌کند:
- ✅ PostgreSQL (Port: 5432)
- ✅ Redis (Port: 6379)
- ✅ pgAdmin (Port: 5050) - برای مدیریت database

### بررسی وضعیت:
```bash
docker-compose -f docker-compose.dev.yml ps
```

### لاگ‌ها:
```bash
docker-compose -f docker-compose.dev.yml logs -f
```

### توقف:
```bash
docker-compose -f docker-compose.dev.yml down
```

---

## گام 2: اجرای Migration

```bash
cd src/SmartPrice.API
dotnet ef database update --project ../SmartPrice.Infrastructure
```

**نکته:** اگر خطا گرفتید، اطمینان حاصل کنید که:
- PostgreSQL در حال اجرا است
- Connection string در `appsettings.Development.json` صحیح است

---

## گام 3: اجرای پروژه

### روش 1: از طریق Visual Studio
1. پروژه `SmartPrice.API` را به عنوان Startup Project انتخاب کنید
2. F5 را بزنید یا Run را کلیک کنید

### روش 2: از طریق Command Line
```bash
cd src/SmartPrice.API
dotnet run
```

### روش 3: با Hot Reload
```bash
cd src/SmartPrice.API
dotnet watch run
```

---

## گام 4: دسترسی به Swagger

پس از اجرای پروژه، به آدرس زیر بروید:
```
http://localhost:5000
یا
https://localhost:5001
```

Swagger UI به طور خودکار باز می‌شود.

---

## گام 5: تست Authentication

### 1. Login Admin

در Swagger:
1. به endpoint `POST /api/auth/login` بروید
2. روی "Try it out" کلیک کنید
3. این داده را وارد کنید:

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

4. "Execute" را بزنید

**پاسخ موفق:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "base64token...",
  "expiresAt": "2025-01-02T00:00:00Z",
  "user": {
    "id": "guid",
    "username": "admin",
    "email": "admin@smartprice.ir",
    "fullName": "System Administrator",
    "role": "SuperAdmin"
  }
}
```

### 2. استفاده از Access Token

1. `accessToken` را کپی کنید
2. در Swagger، روی دکمهٔ **"Authorize"** (قفل سبز بالای صفحه) کلیک کنید
3. در کادر، تایپ کنید: `Bearer {accessToken}` (بدون کروشه)
4. روی "Authorize" کلیک کنید

حالا تمام API های محافظت شده قابل دسترسی هستند!

---

## گام 6: تست Dashboard

### دریافت آمار Dashboard

در Swagger:
1. به endpoint `GET /api/admin/dashboard` بروید
2. "Try it out" و سپس "Execute"

**پاسخ نمونه:**
```json
{
  "totalUsers": 0,
  "activeUsers": 0,
  "totalProducts": 0,
  "trackedProducts": 0,
  "totalScrapingJobs": 0,
  "successfulJobs": 0,
  "failedJobs": 0,
  "notificationsSent": 0,
  "averageResponseTime": 0,
  "userGrowth": [],
  "priceChanges": []
}
```

---

## گام 7: تست سایر Endpoints

### لیست کاربران
```
GET /api/admin/users?page=1&pageSize=20
```

### جزئیات کاربر
```
GET /api/admin/users/{userId}
```

### محصولات محبوب
```
GET /api/admin/products/top?count=10
```

### وضعیت سیستم
```
GET /api/admin/health
```

### Health Check
```
GET /health
```

---

## Troubleshooting

### ❌ خطا: Cannot connect to PostgreSQL

**راه‌حل:**
```bash
# بررسی وضعیت Docker
docker ps

# اگر container اجرا نمی‌شود
docker-compose -f docker-compose.dev.yml up -d postgres

# بررسی لاگ
docker logs smartprice-postgres
```

### ❌ خطا: Migration failed

**راه‌حل:**
```bash
# پاک کردن database و شروع مجدد
docker-compose -f docker-compose.dev.yml down -v
docker-compose -f docker-compose.dev.yml up -d
dotnet ef database update --project ../SmartPrice.Infrastructure
```

### ❌ خطا: Redis connection failed

Redis اختیاری است. اگر خطا داد:
```bash
# راه‌اندازی Redis
docker-compose -f docker-compose.dev.yml up -d redis
```

### ❌ خطا: 401 Unauthorized

**علت:** Token منقضی شده یا نادرست است

**راه‌حل:**
1. دوباره login کنید
2. Token جدید را در Swagger Authorize کنید

### ❌ خطا: Build failed

**راه‌حل:**
```bash
# Clean و Rebuild
dotnet clean
dotnet build
```

---

## اطلاعات مفید

### اطلاعات Admin پیش‌فرض
```
Username: admin
Password: Admin@123
Email: admin@smartprice.ir
```

⚠️ **مهم:** رمز عبور را بعد از اولین ورود تغییر دهید!

### Connection Strings

**PostgreSQL:**
```
Host=localhost;Port=5432;Database=smartprice;Username=postgres;Password=postgres123
```

**Redis:**
```
localhost:6379
```

### pgAdmin Access
```
URL: http://localhost:5050
Email: admin@smartprice.ir
Password: admin123
```

برای اتصال به PostgreSQL در pgAdmin:
- Host: postgres (یا localhost)
- Port: 5432
- Database: smartprice
- Username: postgres
- Password: postgres123

---

## دستورات مفید

### پاک کردن کامل و شروع از نو:
```bash
# توقف تمام containerها و حذف volumes
docker-compose -f docker-compose.dev.yml down -v

# شروع مجدد
docker-compose -f docker-compose.dev.yml up -d

# Migration و Seed
cd src/SmartPrice.API
dotnet ef database update --project ../SmartPrice.Infrastructure
dotnet run
```

### مشاهدهٔ لاگ‌ها:
```bash
# لاگ Docker
docker-compose -f docker-compose.dev.yml logs -f

# لاگ API (در دایرکتوری root)
tail -f logs/smartprice-*.txt
```

---

## نکات مهم

1. ✅ همیشه ابتدا Docker containers را start کنید
2. ✅ اطمینان حاصل کنید Migration اجرا شده است
3. ✅ قبل از تست API ها، حتماً Login و Authorize کنید
4. ✅ برای Development از `appsettings.Development.json` استفاده می‌شود
5. ✅ برای تست از Swagger استفاده کنید (خیلی راحت‌تر از Postman!)

---

## چک‌لیست اجرای موفق

```
□ Docker Desktop نصب و اجرا شده
□ .NET 7 SDK نصب شده
□ PostgreSQL container در حال اجرا
□ Redis container در حال اجرا
□ Migration اجرا شده
□ API با موفقیت start شده
□ Swagger باز می‌شود
□ Login موفقیت‌آمیز
□ Token در Swagger authorize شده
□ Dashboard data دریافت می‌شود
```

---

## حالا چه کنیم؟

پس از اجرای موفقیت‌آمیز:

1. **تست کامل API ها در Swagger**
2. **ایجاد محصولات نمونه**
3. **تست User Management**
4. **بررسی Dashboard Analytics**
5. **تست Refresh Token**
6. **تست Change Password**

---

**موفق باشید! 🎉**

اگر مشکلی پیش آمد، لطفاً:
1. لاگ‌ها را بررسی کنید
2. Connection strings را چک کنید
3. Docker containers را restart کنید
