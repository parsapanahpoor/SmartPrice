# 🐳 اجرای SmartPrice با Docker

## 🚀 شروع سریع (3 گام)

### گام 1: اجرای تمام سرویس‌ها

```powershell
# برای Windows (PowerShell)
.\docker-compose.ps1 -Command up

# یا برای Mac/Linux
./docker-compose.sh up

# یا دستور استاندارد
docker-compose up -d
```

### گام 2: منتظر بمان (30 ثانیه)

سیستم نیاز داره که:
- ✅ PostgreSQL شروع شه
- ✅ Redis شروع شه
- ✅ Seq شروع شه
- ✅ API ساخته شه و شروع شه

### گام 3: مرورگر رو باز کن

```
http://localhost:5000/swagger
```

**انتهایی!** 🎉 برنامه اجرا میشه!

---

## 📊 وضعیت سرویس‌ها

برای چک کردن که همه سرویس‌ها سالم هستن:

```powershell
.\docker-compose.ps1 -Command status
```

یا:
```bash
./docker-compose.sh status
```

یا:
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

---

## 🌐 دسترسی به سرویس‌ها

| سرویس | URL / Address |
|-------|--------------|
| **Swagger UI** | http://localhost:5000/swagger |
| **Health Check** | http://localhost:5000/health |
| **API** | http://localhost:5000/api/products |
| **Seq (Logs)** | http://localhost:5341 |
| **PostgreSQL** | localhost:5432<br/>User: postgres<br/>Password: admin123 |
| **Redis** | localhost:6379 |

---

## 🎛️ دستورات مفید

### Windows (PowerShell)

```powershell
# نمایش راهنما
.\docker-compose.ps1 -Command help

# ساخت images
.\docker-compose.ps1 -Command build

# اجرا
.\docker-compose.ps1 -Command up

# متوقف کردن
.\docker-compose.ps1 -Command down

# دیدن لاگ‌ها
.\docker-compose.ps1 -Command logs

# بررسی وضعیت
.\docker-compose.ps1 -Command ps

# Database migration
.\docker-compose.ps1 -Command migrate

# حذف کامل
.\docker-compose.ps1 -Command clean

# ساخت از صفر
.\docker-compose.ps1 -Command rebuild

# ورود به container‌های مختلف
.\docker-compose.ps1 -Command shell-api
.\docker-compose.ps1 -Command shell-postgres
```

### Linux / Mac

```bash
# نمایش راهنما
./docker-compose.sh help

# ساخت images
./docker-compose.sh build

# اجرا
./docker-compose.sh up

# متوقف کردن
./docker-compose.sh down

# دیدن لاگ‌ها
./docker-compose.sh logs

# بررسی وضعیت
./docker-compose.sh ps

# و ... بقیه دستورات مانند بالا
```

### دستورات استاندارد Docker Compose

```bash
# ساخت
docker-compose build

# اجرا
docker-compose up -d

# متوقف
docker-compose down

# لاگ‌ها
docker-compose logs -f api
docker-compose logs -f postgres
docker-compose logs -f redis

# Restart
docker-compose restart

# Exec دستور در container
docker-compose exec api dotnet ef database update
```

---

## 🔍 نگاه به لاگ‌ها

### لاگ‌های API

```powershell
.\docker-compose.ps1 -Command logs
```

### لاگ‌های تمام سرویس‌ها

```bash
docker-compose logs -f
```

### لاگ‌های مخصوص

```bash
# PostgreSQL
docker-compose logs -f postgres

# Redis
docker-compose logs -f redis

# Seq
docker-compose logs -f seq
```

---

## 🧪 تست کردن API

### با Swagger UI

1. برو به: http://localhost:5000/swagger
2. روی **POST /api/products** کلیک کن
3. **Try it out** رو کلیک کن
4. این JSON رو پیست کن:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "تست محصول",
  "url": "https://test.com/product",
  "imageUrl": "https://test.com/image.jpg",
  "category": "الکترونیک",
  "currentPrice": 1000000,
  "originalPrice": 1200000,
  "discountPercentage": 17,
  "isAvailable": true,
  "lastUpdated": "2024-12-21T00:00:00",
  "createdAt": "2024-12-21T00:00:00",
  "priceHistory": []
}
```

5. **Execute** رو کلیک کن
6. باید **Response 201** ببینی ✅

### با cURL (PowerShell)

```powershell
$json = @{
    name = "تست"
    url = "https://test.com/1"
    imageUrl = "https://test.com/img.jpg"
    category = "الکترونیک"
    currentPrice = 1000000
    originalPrice = 1200000
    discountPercentage = 17
    isAvailable = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/products" `
    -Method Post `
    -ContentType "application/json" `
    -Body $json
```

---

## 🐛 حل مشکلات

### مشکل: "The compose file not found"

```bash
# مطمئن شو در پوشه اصلی پروژه هستی
cd D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode

# سپس دستور رو اجرا کن
docker-compose up -d
```

### مشکل: "Port 5432 is already in use"

```bash
# تغییر پورت در docker-compose.yml
# postgresql:
#   ports:
#     - "5433:5432"  # ← عوض کن
```

### مشکل: "Permission denied while trying to connect to Docker daemon"

#### Windows:
- Docker Desktop رو restart کن

#### Linux:
```bash
sudo usermod -aG docker $USER
# و دوباره لاگین کن
```

### مشکل: API به Database متصل نیست

```bash
# چک کن PostgreSQL healthy هست
docker-compose ps

# دیدن لاگ‌های API
docker-compose logs api

# اگه خطایی بود، restart کن
docker-compose restart postgres
docker-compose restart api
```

### مشکل: Volumes پر شدن

```bash
# حذف volumes نیازی
docker volume prune

# یا حذف کامل
docker-compose down -v
docker-compose up -d
```

---

## 💾 Backup و Restore

### ساخت Backup

```bash
docker-compose exec postgres pg_dump -U postgres smartprice > backup.sql
```

فایل `backup.sql` ساخته میشه.

### Restore از Backup

```bash
cat backup.sql | docker-compose exec -T postgres psql -U postgres -d smartprice
```

---

## 🔐 تغییر رمز عبور

برای تغییر رمز PostgreSQL:

1. فایل `.env` رو باز کن:
```env
POSTGRES_PASSWORD=your-new-password
```

2. یا `docker-compose.yml`:
```yaml
environment:
  POSTGRES_PASSWORD: your-new-password
```

3. و Connection String در `appsettings.Docker.json`:
```json
"DefaultConnection": "Host=postgres;Database=smartprice;Username=postgres;Password=your-new-password"
```

4. Rebuild کن:
```bash
docker-compose down -v
docker-compose up -d
```

---

## 📁 ساختار فایل‌های Docker

```
SmartPrice/
├── Dockerfile                      ← تعریف API
├── docker-compose.yml              ← تمام سرویس‌ها
├── docker-compose.ps1              ← اسکریپت Windows
├── docker-compose.sh               ← اسکریپت Linux/Mac
├── .env                            ← متغیرهای محیط
├── .dockerignore                   ← فایل‌های نامهم
├── Makefile                        ← دستورات Linux
├── src/
│   └── SmartPrice.API/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── appsettings.Docker.json  ← برای Docker
│       └── Program.cs
└── docs/
    ├── DOCKER.md                    ← مستندات Docker
    └── README-DOCKER.md             ← این فایل
```

---

## ✅ چک‌لیست اولیه

قبل از شروع:

- [ ] Docker Desktop نصب است
- [ ] تمام فایل‌های Docker موجود است
- [ ] پوشه `SourceCode` مقابل‌المقدم است
- [ ] فایل `.env` موجود است
- [ ] فایل `docker-compose.yml` موجود است

بعد از اجرا:

- [ ] `docker-compose up -d` تکمیل شد
- [ ] `docker-compose ps` نشون میده healthy
- [ ] Swagger باز میشه: http://localhost:5000/swagger
- [ ] Health Check OK: http://localhost:5000/health
- [ ] می‌تونی محصول اضافه کنی

---

## 🎓 مطالب بیشتر

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Guide](https://docs.docker.com/compose/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
- [Redis Docker Image](https://hub.docker.com/_/redis)
- [Seq Logging](https://datalust.co/seq)

---

## 💬 توجهات

1. **تولید اول Image:** اولین بار ساخت image حدود 2-3 دقیقه طول میکشه
2. **Port Binding:** اگه پورت‌های 5000, 5341, 5432, 6379 اشغال بود، باید عوض کنی
3. **Disk Space:** حدود 2-3 GB برای images و containers
4. **Network:** تمام سرویس‌ها یک network اختصاصی داری (`smartprice-network`)
5. **Health Check:** هر سرویس یک health check داره برای اطمینان از کارکرد

---

**حالا داکرایزیشن کامل است!** 🚀

اگه مشکلی بود، بهم بگو! 💪
