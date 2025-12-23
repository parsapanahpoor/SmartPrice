# 🚀 راهنمای اجرای SmartPrice با Docker

## 📋 خلاصه: چه ساختیم؟

### ✅ فایل‌های Docker اضافه شده:

1. **Dockerfile** - تعریف API Container
2. **docker-compose.yml** - تعریف تمام سرویس‌ها (PostgreSQL, Redis, API, Seq)
3. **docker-compose.ps1** - اسکریپت Windows برای اجرا
4. **docker-compose.sh** - اسکریپت Linux/Mac برای اجرا
5. **.env** - متغیرهای محیط
6. **.dockerignore** - فایل‌های غیر ضروری
7. **Makefile** - دستورات سریع (Linux)
8. **appsettings.Docker.json** - تنظیمات Docker
9. **Program.cs** - به‌روز شده برای Docker

---

## 🎯 سرویس‌های Docker

| سرویس | نقش | Image | Port |
|--------|------|-------|------|
| **PostgreSQL** | دیتابیس اصلی | postgres:16 | 5432 |
| **Redis** | کش | redis:7-alpine | 6379 |
| **SmartPrice API** | برنامه اصلی | (custom) | 5000 |
| **Seq** | مدیریت لاگ‌ها | datalust/seq | 5341 |

---

## 🚀 شروع سریع (3 دستور)

### 1. رفتن به پوشه پروژه

```powershell
cd D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode
```

### 2. اجرای تمام سرویس‌ها

```powershell
# Windows (PowerShell)
.\docker-compose.ps1 -Command up

# یا دستور معمولی
docker-compose up -d
```

### 3. منتظر بمان (30 ثانیه)

تا تمام سرویس‌ها شروع شن و Healthy بشن.

---

## 🌐 دسترسی پس از اجرا

| محل | URL |
|------|-----|
| **Swagger UI** (بهترین!) | http://localhost:5000/swagger |
| **Health Check** | http://localhost:5000/health |
| **API مستقیم** | http://localhost:5000/api/products |
| **Seq (لاگ‌ها)** | http://localhost:5341 |
| **PostgreSQL** | localhost:5432 (user: postgres, pass: admin123) |
| **Redis** | localhost:6379 |

---

## 🎛️ دستورات بیشتر

### Windows (PowerShell)

```powershell
# نمایش تمام دستورات
.\docker-compose.ps1 -Command help

# متوقف کردن
.\docker-compose.ps1 -Command down

# دیدن لاگ‌ها
.\docker-compose.ps1 -Command logs

# بررسی وضعیت
.\docker-compose.ps1 -Command status

# Restart
.\docker-compose.ps1 -Command restart

# حذف کامل
.\docker-compose.ps1 -Command clean

# ساخت از صفر
.\docker-compose.ps1 -Command rebuild

# ورود به container
.\docker-compose.ps1 -Command shell-api
.\docker-compose.ps1 -Command shell-postgres
```

### دستورات استاندارد Docker

```bash
# ساخت images
docker-compose build

# اجرا
docker-compose up -d

# متوقف
docker-compose down

# لاگ‌ها
docker-compose logs -f api

# وضعیت
docker-compose ps

# restart
docker-compose restart
```

---

## 🧪 تست کردن API

### روش 1: Swagger UI (ساده‌ترین)

1. برو به: **http://localhost:5000/swagger**
2. روی **POST /api/products** کلیک کن
3. **Try it out** رو کلیک کن
4. این JSON رو پیست کن:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "گوشی تست",
  "url": "https://test.com/product",
  "imageUrl": "https://test.com/image.jpg",
  "category": "موبایل",
  "currentPrice": 2000000,
  "originalPrice": 2500000,
  "discountPercentage": 20,
  "isAvailable": true,
  "lastUpdated": "2024-12-22T00:00:00",
  "createdAt": "2024-12-22T00:00:00",
  "priceHistory": []
}
```

5. **Execute** رو کلیک کن
6. باید **Response 201** ببینی ✅

### روش 2: PowerShell

```powershell
# تعریف داده‌ها
$json = @{
    name = "لپ تاپ"
    url = "https://test.com/laptop"
    imageUrl = "https://test.com/laptop.jpg"
    category = "کامپیوتر"
    currentPrice = 50000000
    originalPrice = 60000000
    discountPercentage = 15
    isAvailable = $true
} | ConvertTo-Json

# ارسال
Invoke-RestMethod -Uri "http://localhost:5000/api/products" `
    -Method Post `
    -ContentType "application/json" `
    -Body $json
```

---

## 🔍 چک کردن وضعیت

### تمام سرویس‌ها سالم هستن؟

```bash
docker-compose ps
```

باید این رو ببینی:

```
NAME                  STATUS          PORTS
smartprice-postgres   Up (healthy)    5432/tcp
smartprice-redis      Up (healthy)    6379/tcp
smartprice-api        Up (healthy)    5000/tcp
smartprice-seq        Up              5341/tcp
```

### سلامت API

```bash
curl http://localhost:5000/health
```

باید ببینی: **Healthy** ✅

---

## 🐛 حل مشکلات

### مشکل 1: "docker-compose: command not found"

Docker Desktop نصب نشده یا اجرا نمی‌شود.

**حل:**
```powershell
# چک کن Docker اجرا است
docker --version

# اگر نیست، Docker Desktop رو باز کن
# شروع بر روی Windows: کلیک بر روی Docker Desktop در Start Menu
```

### مشکل 2: "Port 5000 is already in use"

```
error: listen EADDRINUSE: address already in use :::5000
```

**حل:**
- برنامه دیگری از پورت 5000 استفاده میکنه
- یا پورت رو تغییر دهید در `docker-compose.yml`:

```yaml
ports:
  - "5050:5000"  # ← 5000 رو 5050 کن
```

### مشکل 3: API به Database متصل نیست

```
Error: Cannot find database "smartprice"
```

**حل:**
```bash
# چک کن PostgreSQL healthy هست
docker-compose ps

# ببین لاگ‌های API
docker-compose logs api

# اگه خطا بود، restart کن
docker-compose restart postgres
docker-compose restart api
```

### مشکل 4: Containers اجرا نشده

```bash
docker-compose down
docker-compose up -d --build
```

---

## 📊 نگاه به لاگ‌ها

```bash
# لاگ‌های API
docker-compose logs -f api

# لاگ‌های PostgreSQL
docker-compose logs -f postgres

# لاگ‌های Redis
docker-compose logs -f redis

# لاگ‌های Seq
docker-compose logs -f seq

# تمام لاگ‌ها
docker-compose logs -f
```

---

## 💾 Backup و Restore

### ساخت Backup

```bash
docker-compose exec postgres pg_dump -U postgres smartprice > backup.sql
```

### Restore

```bash
cat backup.sql | docker-compose exec -T postgres psql -U postgres -d smartprice
```

---

## 🔐 تغییر رمز عبور

اگه می‌خوای رمز PostgreSQL رو عوض کنی:

1. فایل `.env` رو باز کن
2. تغییر بده:
```env
POSTGRES_PASSWORD=your-new-password
```

3. و فایل `appsettings.Docker.json`:
```json
"DefaultConnection": "Host=postgres;Database=smartprice;Username=postgres;Password=your-new-password"
```

4. Rebuild کن:
```bash
docker-compose down -v
docker-compose up -d
```

---

## ✅ چک‌لیست نهایی

قبل از تکمیل:

- [ ] Docker Desktop نصب است
- [ ] `docker-compose up -d` تکمیل شد
- [ ] `docker-compose ps` نشون میده Healthy
- [ ] Swagger باز میشه: http://localhost:5000/swagger
- [ ] Health Check OK: http://localhost:5000/health
- [ ] می‌تونی محصول اضافه کنی
- [ ] محصول در دیتابیس ذخیره میشه

---

## 📚 فایل‌های مهم

| فایل | توضیح |
|------|---------|
| `Dockerfile` | تعریف API Image |
| `docker-compose.yml` | تمام سرویس‌ها |
| `appsettings.Docker.json` | تنظیمات Docker |
| `.env` | متغیرهای محیط |
| `README-DOCKER.md` | مستندات کامل |
| `DOCKER-SUMMARY.md` | خلاصه فایل‌ها |

---

## 🎓 یادگیری بیشتر

اگه می‌خوای بیشتر یاد بگیری:
- `docs/DOCKER.md` - مستندات کامل
- `README-DOCKER.md` - راهنمای سریع
- `DOCKER-SUMMARY.md` - خلاصه فایل‌ها

---

## 💡 نکات مهم

1. **اولین اجرا:** 2-3 دقیقه طول میکشه (برای ساخت image)
2. **بعدی‌ها:** فقط چند ثانیه
3. **Disk Space:** حدود 2 GB
4. **Memory:** حدود 500 MB-1 GB
5. **Network:** تمام سرویس‌ها یک network اختصاصی داری

---

## 🎉 حالا آماده‌ای!

```
✅ Docker setup کامل
✅ تمام سرویس‌ها آماده
✅ API اجرا شده
✅ دیتابیس تنظیم شده
✅ Logging فعال
```

**یک دستور برای اجرای کل پروژه:**

```bash
docker-compose up -d
```

**به بهت روز!** 🚀
