# 🔧 حل مشکل Migration - Exception has been thrown by the target of an invocation

## ❌ مشکل
هنگام اجرای دستور Migration:
```bash
dotnet ef database update --project ..\SmartPrice.Infrastructure
```

این خطا را دریافت کردید:
```
Exception has been thrown by the target of an invocation.
```

---

## ✅ راه‌حل 1: Migration بدون Seeding (پیشنهادی)

این مشکل رفع شده است! حالا می‌توانید به راحتی Migration اجرا کنید:

### گام 1: اطمینان از اجرای PostgreSQL
```bash
docker-compose -f docker-compose.dev.yml up -d postgres
```

### گام 2: بررسی وضعیت
```bash
docker ps
```

باید `smartprice-postgres` را ببینید.

### گام 3: اجرای Migration
```bash
cd src\SmartPrice.API
dotnet ef database update --project ..\SmartPrice.Infrastructure
```

### گام 4: اجرای API
```bash
dotnet run
```

Admin user به صورت خودکار هنگام اولین اجرای API ایجاد می‌شود! 🎉

---

## ✅ راه‌حل 2: پاک کردن و شروع از نو

اگر هنوز مشکل دارید:

### پاک کردن کامل Database
```bash
# توقف و حذف تمام containers و volumes
docker-compose -f docker-compose.dev.yml down -v

# شروع مجدد
docker-compose -f docker-compose.dev.yml up -d

# صبر 10 ثانیه تا PostgreSQL آماده شود
```

### اجرای Migration
```bash
cd src\SmartPrice.API
dotnet ef database update --project ..\SmartPrice.Infrastructure
```

---

## ✅ راه‌حل 3: Migration دستی

اگر هنوز کار نمی‌کند، Migration را دستی ایجاد کنید:

### حذف Migrations قدیمی
```bash
cd src\SmartPrice.Infrastructure
rm -r Migrations
```

### ایجاد Migration جدید
```bash
cd ..\SmartPrice.API
dotnet ef migrations add InitialCreate --project ..\SmartPrice.Infrastructure
dotnet ef database update --project ..\SmartPrice.Infrastructure
```

---

## 🔍 بررسی مشکلات احتمالی

### 1. PostgreSQL در حال اجرا نیست
```bash
docker ps

# اگر smartprice-postgres را نمی‌بینید:
docker-compose -f docker-compose.dev.yml up -d postgres
docker logs smartprice-postgres
```

### 2. Connection String نادرست است
بررسی کنید در `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=smartprice;Username=postgres;Password=postgres123"
  }
}
```

### 3. Port 5432 قبلاً استفاده می‌شود
```bash
# در Windows PowerShell
netstat -ano | findstr :5432

# اگر چیزی پیدا کردید، یا آن Process را kill کنید یا Port را تغییر دهید
```

### 4. EF Tools نصب نیست
```bash
dotnet tool install --global dotnet-ef
# یا
dotnet tool update --global dotnet-ef
```

---

## 📋 Checklist قبل از Migration

```
□ Docker Desktop در حال اجرا است
□ PostgreSQL container start شده
□ Port 5432 آزاد است (یا در حال استفاده توسط PostgreSQL container)
□ Connection string صحیح است
□ dotnet-ef tools نصب است
□ در دایرکتوری SmartPrice.API هستید
```

---

## 🎯 بعد از Migration موفق

### اجرای API
```bash
dotnet run
```

### Admin اولیه
بعد از start شدن API، این پیام را خواهید دید:
```
[INFO] Default admin user created
[WARN] ⚠️  Default credentials - Username: admin, Password: Admin@123
```

### تست در Swagger
```
http://localhost:5000
```

Login با:
```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

---

## 💡 توضیح مشکل

مشکل اصلی این بود که:
- EF Tools نمی‌تواند async code را در startup handle کند
- DatabaseSeeder از async Task استفاده می‌کرد
- این باعث exception می‌شد

**راه‌حل:**
- Seeding را از Migration جدا کردیم
- Seeding در اولین اجرای API انجام می‌شود (نه هنگام Migration)
- Migration فقط schema را ایجاد می‌کند

---

## 📞 اگر هنوز مشکل دارید

1. **لاگ کامل را ببینید:**
   ```bash
   dotnet ef database update --project ..\SmartPrice.Infrastructure --verbose
   ```

2. **Build را چک کنید:**
   ```bash
   dotnet build
   ```

3. **Connection را تست کنید:**
   ```bash
   docker exec -it smartprice-postgres psql -U postgres
   # باید وارد PostgreSQL shell شوید
   \l  # لیست databases
   \q  # خروج
   ```

---

## ✅ خلاصه

مشکل حل شد! حالا:
1. ✅ Migration بدون مشکل اجرا می‌شود
2. ✅ Admin user هنگام اولین اجرا ایجاد می‌شود
3. ✅ همه چیز آماده است!

**فقط این دستورات را اجرا کنید:**
```bash
# 1. Start Database
docker-compose -f docker-compose.dev.yml up -d

# 2. Migration
cd src\SmartPrice.API
dotnet ef database update --project ..\SmartPrice.Infrastructure

# 3. Run API
dotnet run

# 4. Test in browser
# http://localhost:5000
```

**موفق باشید! 🚀**
