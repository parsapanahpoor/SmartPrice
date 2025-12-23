# 🐳 SmartPrice Docker - خلاصه فایل‌ها

## 📦 فایل‌های اضافه شده

### 1. **Dockerfile**
- تعریف Image برای SmartPrice API
- Build stage: ساخت پروژه
- Publish stage: انتشار binary
- Runtime stage: اجرای برنامه
- Health check شامل

### 2. **docker-compose.yml**
- تعریف تمام سرویس‌ها:
  - PostgreSQL 16 (دیتابیس)
  - Redis 7 (کش)
  - SmartPrice API (برنامه اصلی)
  - Seq (لاگ‌ها)
- Network اختصاصی
- Health checks
- Volumes برای ذخیره دائمی

### 3. **docker-compose.ps1** (Windows)
- اسکریپت PowerShell برای ویندوز
- دستورات سریع و راحت
- رنگی و کاربرپسند
- تابع‌های کمکی

### 4. **docker-compose.sh** (Linux/Mac)
- اسکریپت Bash برای Linux و Mac
- دستورات مشابه PowerShell
- رنگی و خوب‌ترتیب‌یافته
- portable

### 5. **.env**
- متغیرهای محیط
- رمزهای پیشفرض (برای توسعه)
- تنظیمات سرویس‌ها

### 6. **appsettings.Docker.json**
- تنظیمات ویژه برای Docker
- Connection strings درست
- Seq URL صحیح (http://seq:5341)
- Redis connection

### 7. **Makefile** (برای Linux/Mac)
- دستورات سریع اختیاری
- برای کاربران Linux/Mac

### 8. **docs/DOCKER.md**
- مستندات کامل Docker
- تمام دستورات
- حل مشکلات
- نکات مهم

### 9. **README-DOCKER.md**
- راهنمای سریع شروع
- دسترسی‌های سریع
- مثال‌های عملی
- تست کردن API

### 10. **Program.cs** (تغییر‌یافته)
- پشتیبانی محیط Docker
- شناسایی Seq URL
- بهتر کردن Redis check
- error handling بهتر

---

## 🎯 نقش هر فایل

| فایل | کاربرد | اختیاری |
|------|--------|---------|
| Dockerfile | ساخت API Image | ❌ الزامی |
| docker-compose.yml | اجرای تمام سرویس‌ها | ❌ الزامی |
| .dockerignore | فایل‌های نادیده | ❌ الزامی |
| .env | متغیرهای محیط | ⚠️ توصیه‌شده |
| docker-compose.ps1 | اسکریپت Windows | ✅ اختیاری |
| docker-compose.sh | اسکریپت Linux | ✅ اختیاری |
| Makefile | دستورات Linux | ✅ اختیاری |
| appsettings.Docker.json | تنظیمات Docker | ⚠️ توصیه‌شده |
| docs/DOCKER.md | مستندات | ✅ مرجع |
| README-DOCKER.md | راهنما | ✅ مرجع |

---

## 🚀 فلوی اجرا

```
1. docker-compose up -d
   ↓
2. Docker Compose فایل‌ها رو پارس میکنه
   ↓
3. Network ساخته میشه (smartprice-network)
   ↓
4. PostgreSQL Container شروع میشه
   ├─ Volumes mount میشه (postgres_data)
   └─ Health check شروع میشه
   ↓
5. Redis Container شروع میشه
   ├─ Volumes mount میشه (redis_data)
   └─ Health check شروع میشه
   ↓
6. Seq Container شروع میشه
   └─ Volumes mount میشه (seq_data)
   ↓
7. API Container شروع میشه
   ├─ Dockerfile execute میشه
   ├─ Dependencies restore میشه
   ├─ Build میشه
   ├─ Publish میشه
   ├─ اجرا میشه
   └─ Health check شروع میشه
   ↓
8. تمام سرویس‌ها Healthy هستن
   ↓
✅ Ready!
```

---

## 🔗 سرویس‌های شبکه

```
smartprice-network
├── postgres:5432 (Internal)
├── redis:6379 (Internal)
├── api:5000 (External: 5000:5000)
└── seq:80 (External: 5341:80)
```

---

## 📊 Container‌های ساخته‌شده

```
1. smartprice-postgres
   ├─ Image: postgres:16
   ├─ Port: 5432:5432
   ├─ Database: smartprice
   └─ Health: pg_isready

2. smartprice-redis
   ├─ Image: redis:7-alpine
   ├─ Port: 6379:6379
   └─ Health: redis-cli ping

3. smartprice-api
   ├─ Image: smartprice:latest (ساخته‌شده)
   ├─ Port: 5000:5000
   ├─ Environment: Docker
   └─ Health: curl /health

4. smartprice-seq
   ├─ Image: datalust/seq
   ├─ Port: 5341:80
   └─ Health: curl /health
```

---

## 💾 Volumes

```
postgres_data
├─ نقش: ذخیره دیتابیس PostgreSQL
├─ مسیر Container: /var/lib/postgresql/data
└─ پایدار: ✅ بعد از delete

redis_data
├─ نقش: ذخیره snapshot Redis
├─ مسیر Container: /data
└─ پایدار: ✅ بعد از delete

seq_data
├─ نقش: ذخیره لاگ‌های Seq
├─ مسیر Container: /data
└─ پایدار: ✅ بعد از delete
```

---

## 🌍 متغیرهای محیط

```
POSTGRES_USER: postgres
POSTGRES_PASSWORD: admin123
POSTGRES_DB: smartprice

ASPNETCORE_ENVIRONMENT: Docker
API_PORT: 5000

REDIS_HOST: redis
REDIS_PORT: 6379

SEQ_HOST: seq
SEQ_PORT: 5341
```

---

## 🔐 Security Notes

⚠️ **یادداشت**: این تنظیمات برای **توسعه** است، برای Production:

1. رمزهای قوی استفاده کن
2. Volumes رو Encrypt کن
3. Networks رو محدود کن
4. Environment Variables رو Secret Management رو استفاده کن
5. API رو بدون Swagger Deploy کن

---

## 📈 Performance Considerations

- **Build Time:** اولین بار 2-3 دقیقه (بعدها فقط چند ثانیه)
- **Disk Space:** ~2-3 GB برای images و containers
- **Memory:** ~500 MB-1 GB تقریبی
- **Startup Time:** ~30 ثانیه تا تمام سرویس‌ها healthy شن

---

## ✅ چک‌لیست پیاده‌سازی

- [x] Dockerfile ساخته شده
- [x] docker-compose.yml ساخته شده
- [x] PostgreSQL سرویس اضافه شده
- [x] Redis سرویس اضافه شده
- [x] Seq سرویس اضافه شده
- [x] API سرویس اضافه شده
- [x] Health checks اضافه شده
- [x] Networks تعریف شده
- [x] Volumes تعریف شده
- [x] appsettings.Docker.json ساخته شده
- [x] Program.cs به‌روز شده
- [x] اسکریپت‌های Windows ساخته شده
- [x] اسکریپت‌های Linux ساخته شده
- [x] Makefile ساخته شده
- [x] مستندات نوشته شده
- [x] .dockerignore ساخته شده
- [x] .env ساخته شده

---

## 🎓 دستورات کلیدی

### ساخت و اجرا

```bash
# بدون اسکریپت
docker-compose build
docker-compose up -d

# با اسکریپت Windows
.\docker-compose.ps1 -Command up

# با اسکریپت Linux
./docker-compose.sh up
```

### مراقبت

```bash
docker-compose ps           # وضعیت
docker-compose logs -f api  # لاگ‌ها
docker-compose restart      # restart
docker-compose down         # متوقف
```

### Cleanup

```bash
docker-compose down -v      # حذف همه (volumes هم)
```

---

## 📞 توجهات نهایی

1. ✅ **نام Container‌ها:** ثابت و معنی‌دار (`smartprice-*`)
2. ✅ **Port Mapping:** تمام پورت‌ها معیاری و documented
3. ✅ **Health Checks:** هر سرویس health check داره
4. ✅ **Volumes:** داده‌ها پایدار هستن
5. ✅ **Network:** سرویس‌ها در یک network محفوظ
6. ✅ **Logging:** تمام output مراقبت‌شده
7. ✅ **Error Handling:** بهترین تلاش برای error recovery
8. ✅ **Documentation:** تمام فایل‌ها documented هستن

---

**داکرایزیشن SmartPrice کامل است!** 🚀

حالا می‌تونی با یک دستور ساده تمام پروژه رو اجرا کنی!
