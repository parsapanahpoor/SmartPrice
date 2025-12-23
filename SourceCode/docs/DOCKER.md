# 🐳 Docker و Docker Compose برای SmartPrice

## 📋 نصب‌شده‌ها

این پروژه شامل Docker setup برای:
- ✅ **PostgreSQL 16** - دیتابیس اصلی
- ✅ **Redis 7** - کش‌ کردن داده‌ها
- ✅ **SmartPrice API** - برنامه اصلی
- ✅ **Seq** - مدیریت لاگ‌ها

---

## 🚀 شروع سریع

### 1. ساخت و اجرای تمام سرویس‌ها

```bash
docker-compose up -d
```

یا اگه Makefile داری:

```bash
make up
```

### 2. منتظر بمون (30 ثانیه)

باید تمام سرویس‌ها healthy بشن:

```bash
docker-compose ps
```

### 3. بازدید سایت‌ها

- **Swagger UI**: http://localhost:5000/swagger
- **API**: http://localhost:5000/api/products
- **Health**: http://localhost:5000/health
- **Seq**: http://localhost:5341
- **Database**: `localhost:5432` (postgres / admin123)
- **Redis**: `localhost:6379`

---

## 📦 ساختار فایل‌ها

```
SmartPrice/
├── Dockerfile                    ← تعریف API Image
├── docker-compose.yml            ← تمام سرویس‌ها
├── .dockerignore                 ← فایل‌های غیر ضروری
├── .env                          ← متغیرهای محیط
├── Makefile                      ← دستورات سریع
├── src/
│   └── SmartPrice.API/
│       ├── appsettings.json      ← تنظیمات معمولی
│       └── appsettings.Docker.json ← تنظیمات Docker
└── docs/
    └── DOCKER.md                 ← این فایل
```

---

## 🎛️ دستورات اصلی

### ساخت و اجرا

```bash
# ساخت images
docker-compose build

# اجرای تمام سرویس‌ها
docker-compose up -d

# اجرای با دیدن لاگ‌ها
docker-compose up
```

### متوقف کردن و حذف

```bash
# متوقف کردن (داده‌ها باقی میمن)
docker-compose down

# حذف کامل (شامل داده‌ها)
docker-compose down -v
```

### دیدن لاگ‌ها

```bash
# لاگ‌های API
docker-compose logs -f api

# لاگ‌های PostgreSQL
docker-compose logs -f postgres

# لاگ‌های Redis
docker-compose logs -f redis

# لاگ‌های Seq
docker-compose logs -f seq

# همه لاگ‌ها
docker-compose logs -f
```

### ورود به Container‌ها

```bash
# ورود به API Container
docker-compose exec api sh

# ورود به PostgreSQL
docker-compose exec postgres psql -U postgres -d smartprice

# ورود به Redis
docker-compose exec redis redis-cli
```

---

## 🔧 استفاده از Makefile

اگه Makefile نصب است، این دستورات کار میکن:

```bash
# راهنما
make help

# ساخت و اجرا
make build       # ساخت images
make up          # اجرا
make down        # متوقف کردن
make restart     # restart

# لاگ‌ها
make logs        # لاگ API
make logs-all    # همه لاگ‌ها

# پاکسازی
make clean       # حذف همه
make rebuild     # شروع از صفر

# Migrations
make migrate     # اجرای migrations

# شل‌ها
make shell-api       # ورود به API
make shell-postgres  # ورود به DB
make shell-redis     # ورود به Redis

# وضعیت
make ps          # لیست containers
make status      # وضعیت سلامت
```

---

## 📊 وضعیت سرویس‌ها

### چک کردن Health

```bash
docker-compose ps
```

باید این رو ببینی:

```
NAME                    STATUS           PORTS
smartprice-postgres     Up (healthy)     5432/tcp
smartprice-redis        Up (healthy)     6379/tcp
smartprice-api          Up (healthy)     5000/tcp
smartprice-seq          Up                5341/tcp
```

### Health Check API

```bash
curl http://localhost:5000/health
```

باید `Healthy` برگشت بگیری.

---

## 🐛 رفع مشکلات

### مشکل 1: Containers اجرا نشن

```bash
# چک کن Docker Desktop اجرا است
docker --version

# بررسی errors
docker-compose logs

# دوباره اجرا
docker-compose down
docker-compose up --build
```

### مشکل 2: API به DB متصل نیست

```bash
# چک کن PostgreSQL healthy هست
docker-compose ps

# ببین لاگ‌های API
docker-compose logs api

# Connection String رو چک کن
# باید "Host=postgres" باشه (نه localhost)
```

### مشکل 3: پورت در حال استفاده

```bash
# تغییر پورت در docker-compose.yml
# مثلاً 5000 رو 5050 کن:
# ports:
#   - "5050:5000"
```

### مشکل 4: Volume permission denied

```bash
# اگه محتاج permission
docker-compose down -v
# و دوباره اجرا
docker-compose up -d
```

---

## 💾 Backup و Restore

### Backup دیتابیس

```bash
docker-compose exec postgres pg_dump -U postgres smartprice > backup.sql
```

### Restore دیتابیس

```bash
cat backup.sql | docker-compose exec -T postgres psql -U postgres -d smartprice
```

---

## 🔐 متغیرهای محیط

اینها در `.env` تنظیم میشن:

```env
# Database
POSTGRES_USER=postgres
POSTGRES_PASSWORD=admin123
POSTGRES_DB=smartprice

# Application
ASPNETCORE_ENVIRONMENT=Docker
API_PORT=5000

# Redis
REDIS_HOST=redis
REDIS_PORT=6379

# Seq
SEQ_HOST=seq
SEQ_PORT=5341
```

برای تغییر، فایل `.env` رو ویرایش کن و دوباره اجرا کن.

---

## 📈 مدیریت حجم

### دیدن استفاده Volumes

```bash
docker volume ls
docker volume inspect smartprice-postgres-postgres_data
```

### پاکسازی Volume‌های نیازی

```bash
docker volume prune
```

---

## 🚀 Production Deployment

برای استفاده در Production:

1. **تغییر رمز عبور**:
```bash
# در .env یا docker-compose.yml
POSTGRES_PASSWORD=your-strong-password-here
```

2. **تغییر ASPNETCORE_ENVIRONMENT**:
```bash
ASPNETCORE_ENVIRONMENT=Production
```

3. **غیرفعال کردن Swagger**:
```csharp
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

4. **تنظیم HTTPS**:
```bash
# اضافه کردن certificate
volumes:
  - ./certs:/app/certs
```

---

## 📚 منابع

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
- [Redis Docker Image](https://hub.docker.com/_/redis)
- [Seq Docker Image](https://hub.docker.com/r/datalust/seq)

---

## ✅ چک‌لیست اجرا

- [ ] Docker Desktop نصب و اجرا است
- [ ] Makefile یا docker-compose موجود است
- [ ] `.env` فایل موجود است
- [ ] `docker-compose up -d` اجرا شده
- [ ] `docker-compose ps` نشون میده healthy
- [ ] Swagger باز میشه: http://localhost:5000/swagger
- [ ] Health Check OK: http://localhost:5000/health
- [ ] Seq نشون میده لاگ‌ها: http://localhost:5341
- [ ] PostgreSQL اتصال دارد
- [ ] Redis اتصال دارد

---

**حالا تمام چیز برای Docker آماده است!** 🚀
