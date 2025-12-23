# 🐳 راهنمای Docker - SmartPrice

## فهرست مطالب
- [پیش‌نیازها](#پیش‌نیازها)
- [شروع سریع](#شروع-سریع)
- [دستورات](#دستورات)
- [پیکربندی](#پیکربندی)
- [رفع مشکلات](#رفع-مشکلات)

---

## 📋 پیش‌نیازها

قبل از شروع، این نرم‌افزارها را نصب کنید:

1. **Docker Desktop** (برای Windows/Mac)
   - [دانلود Docker Desktop](https://www.docker.com/products/docker-desktop)
   
2. **Docker Engine** (برای Linux)
   ```bash
   curl -fsSL https://get.docker.com -o get-docker.sh
   sudo sh get-docker.sh
   ```

3. **Docker Compose** (معمولاً با Docker Desktop می‌آید)
   ```bash
   docker-compose --version
   ```

---

## 🚀 شروع سریع

### 1️⃣ کلون کردن پروژه

```bash
git clone https://github.com/parsapanahpoor/SmartPrice.git
cd SmartPrice/SourceCode
```

### 2️⃣ تنظیم متغیرهای محیط

```bash
# کپی کردن نمونه فایل
cp .env.example .env

# ویرایش فایل (و اضافه کردن توکن تلگرام)
nano .env
```

یا برای Windows:
```powershell
copy .env.example .env
notepad .env
```

### 3️⃣ اجرای Docker Compose (Development)

```bash
# اجرای تمام سرویس‌ها
docker-compose up -d

# مشاهده لاگ‌ها
docker-compose logs -f

# متوقف کردن
docker-compose down
```

### 4️⃣ اجرای Docker Compose (Production)

```bash
# اجرای برای محیط تولید
docker-compose -f docker-compose.prod.yml up -d

# متوقف کردن
docker-compose -f docker-compose.prod.yml down
```

---

## 📝 دستورات کاربردی

### مشاهده وضعیت سرویس‌ها

```bash
# وضعیت کنتینرها
docker-compose ps

# لاگ‌های یک سرویس خاص
docker-compose logs api
docker-compose logs postgres
docker-compose logs redis
docker-compose logs seq
```

### اتصال به دیتابیس

```bash
# اتصال به PostgreSQL
docker-compose exec postgres psql -U postgres -d smartprice

# دیدن جداول
\dt

# خروج
\q
```

### اتصال به Redis

```bash
# اتصال به Redis
docker-compose exec redis redis-cli

# مشاهده کلیدها
KEYS *

# خروج
exit
```

### مدیریت دیتابیس

```bash
# Backup دیتابیس
docker-compose exec postgres pg_dump -U postgres smartprice > backup.sql

# Restore دیتابیس
docker-compose exec -T postgres psql -U postgres smartprice < backup.sql

# حذف تمام داده‌ها
docker-compose exec postgres psql -U postgres smartprice -c "TRUNCATE ALL TABLES CASCADE;"
```

### اجرای Migration

```bash
# اجرای migration در کنتینر
docker-compose exec api dotnet ef database update --project ../src/SmartPrice.Infrastructure

# مشاهده migration‌ها
docker-compose exec api dotnet ef migrations list
```

---

## ⚙️ پیکربندی

### فایل `.env`

تمام متغیرهای محیط در `.env` تعریف می‌شوند:

```env
# Database
DB_USER=postgres
DB_PASSWORD=admin123
DB_NAME=smartprice

# Redis
REDIS_PASSWORD=redis123

# Telegram
TELEGRAM_BOT_TOKEN=YOUR_TOKEN
TELEGRAM_CHANNEL_ID=@channel

# Logging
SERILOG_MIN_LEVEL=Information
```

### فایل `docker-compose.yml` (Development)

سرویس‌های Development:
- **PostgreSQL**: پایگاه داده
- **Redis**: کش کردن
- **API**: برنامه‌ی اصلی
- **Seq**: لاگ‌ها

### فایل `docker-compose.prod.yml` (Production)

تنظیمات بهینه برای Production:
- بیشتر Health Check
- Logging
- Restart Policy
- Password Protection
- Resource Limits (توصیه)

---

## 📊 معمارِ Docker

```
┌─────────────────────────────────────┐
│      Docker Compose Network         │
├─────────────────────────────────────┤
│                                     │
│  ┌──────────────────────────────┐  │
│  │   SmartPrice API             │  │
│  │   (Port: 5000, 5001)         │  │
│  └──────────────────────────────┘  │
│           ↓      ↓      ↓           │
│  ┌─────────────────────────────┐   │
│  │   PostgreSQL (5432)         │   │
│  │   Redis (6379)              │   │
│  │   Seq (5341)                │   │
│  └─────────────────────────────┘   │
│                                     │
└─────────────────────────────────────┘
```

---

## 🔍 رفع مشکلات

### مشکل 1: "Cannot connect to Docker daemon"

**خطا:**
```
error during connect: This error may indicate that the docker daemon is not running
```

**راه حل:**
- Docker Desktop را شروع کنید
- یا Docker Service را شروع کنید (Linux)

### مشکل 2: "Port already in use"

**خطا:**
```
Error response from daemon: Ports are not available
```

**راه حل:**
```bash
# نمایش پروسس‌های استفاده کننده پورت
lsof -i :5000  # برای Mac/Linux
netstat -ano | findstr :5000  # برای Windows

# یا تغییر پورت در docker-compose.yml
ports:
  - "5001:5000"  # port 5000 → 5001
```

### مشکل 3: Database connection failed

**خطا:**
```
Failed to connect to postgres
```

**راه حل:**
```bash
# بررسی وضعیت PostgreSQL
docker-compose ps postgres

# مشاهده لاگ‌های PostgreSQL
docker-compose logs postgres

# دوباره شروع کردن
docker-compose restart postgres
```

### مشکل 4: API not starting

**خطا:**
```
Application startup failed
```

**راه حل:**
```bash
# مشاهده لاگ‌های API
docker-compose logs -f api

# دوباره بیلد کردن
docker-compose down
docker-compose build
docker-compose up -d
```

### مشکل 5: Migration failed

**خطا:**
```
The migration 'XXX' has not been applied to the database
```

**راه حل:**
```bash
# اجرای manual migration
docker-compose exec api dotnet ef database update --project ../src/SmartPrice.Infrastructure

# یا حذف دیتابیس و شروع دوباره
docker-compose down -v
docker-compose up -d
```

---

## 🌐 دسترسی به سرویس‌ها

### API

```
http://localhost:5000
https://localhost:5001
```

**مثال:**
```bash
# Health Check
curl http://localhost:5000/health

# Swagger
http://localhost:5000/swagger
```

### Seq (Logging)

```
http://localhost:5341
```

مشاهده لاگ‌های real-time

### PostgreSQL

```
Host: localhost
Port: 5432
User: postgres
Password: admin123 (یا مقدار .env)
Database: smartprice
```

### Redis

```
Host: localhost
Port: 6379
```

---

## 📦 Volume ها

Docker Compose سه Volume ایجاد می‌کند:

| Volume | مقصد | مطالب |
|--------|------|-------|
| `postgres_data` | Database | جداول و داده‌ها |
| `redis_data` | Cache | داده‌های کش |
| `seq_data` | Logging | لاگ‌ها |

### پاک کردن Volume ها

```bash
# حذف تمام volume ها (خطرناک!)
docker-compose down -v

# حذف volume خاص
docker volume rm smartprice_postgres_data
```

---

## 🔐 Security Tips

### Development
- ✅ پسورد‌های پیش‌فرض قابل قبول است
- ⚠️ صرفاً برای توسعه محلی

### Production
- ❌ **هرگز** پسورد پیش‌فرض را استفاده نکنید
- ✅ پسورد‌های قوی تعیین کنید
- ✅ متغیرهای محیط را استفاده کنید
- ✅ HTTPS فعال کنید
- ✅ Firewall تنظیم کنید

```bash
# تولید پسورد قوی
openssl rand -base64 32
```

---

## 📊 Monitoring

### Real-time Logs

```bash
docker-compose logs -f
```

### Resource Usage

```bash
docker stats
```

### Container Health

```bash
docker-compose ps
```

---

## 🚀 بهینه‌سازی برای Production

### 1. Resource Limits اضافه کنید

```yaml
api:
  deploy:
    resources:
      limits:
        cpus: '1'
        memory: 512M
      reservations:
        cpus: '0.5'
        memory: 256M
```

### 2. Restart Policy

```yaml
restart: always
restart: on-failure
restart: unless-stopped
```

### 3. Logging Driver

```yaml
logging:
  driver: "json-file"
  options:
    max-size: "10m"
    max-file: "3"
```

### 4. Network Security

```bash
# استفاده از internal network بجای publish کردن تمام پورت‌ها
```

---

## 📚 منابع بیشتر

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Best Practices for Building Docker Images](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)

---

## ❓ سوالات متکررر

**Q: چگونه Container را بکاپ بگیرم؟**
A: 
```bash
docker-compose exec postgres pg_dump -U postgres smartprice > backup.sql
```

**Q: چگونه مقدار متغیر محیط را تغییر دهم؟**
A: `.env` را ویرایش کنید و `docker-compose restart` اجرا کنید

**Q: چگونه Access Log ببینم؟**
A:
```bash
docker-compose logs -f api
```

**Q: چگونه Database را Reset کنم؟**
A:
```bash
docker-compose down -v
docker-compose up -d
```

---

## 📞 پشتیبانی

برای مسائل و سوالات:
- [GitHub Issues](https://github.com/parsapanahpoor/SmartPrice/issues)
- [Documentation](./README.md)

**سازنده**: SmartPrice Team

**نسخه**: 1.0.0

**آخرین بروزرسانی**: 2024
