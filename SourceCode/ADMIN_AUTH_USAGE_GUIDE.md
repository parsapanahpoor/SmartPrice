# 🚀 SmartPrice - Phase 5 & 6 Implementation Guide

## 📋 خلاصه

این راهنمای نحوهٔ استفاده از Admin Panel و Authentication System است که در Phase 5 و 6 پیاده‌سازی شده‌اند.

---

## 🔐 Authentication

### ورود کاربر

```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}'
```

**پاسخ:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "base64EncodedToken",
  "expiresAt": "2025-01-02T00:00:00Z",
  "user": {
    "id": "uuid",
    "username": "admin",
    "email": "admin@example.com",
    "fullName": "Admin User",
    "role": "SuperAdmin"
  }
}
```

### استفاده از Access Token

```bash
curl -X GET "http://localhost:5000/api/admin/dashboard" \
  -H "Authorization: Bearer {accessToken}"
```

### بازتوازن توکن

```bash
curl -X POST "http://localhost:5000/api/auth/refresh" \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"base64EncodedToken"}'
```

### خروج کاربر

```bash
curl -X POST "http://localhost:5000/api/auth/logout" \
  -H "Authorization: Bearer {accessToken}"
```

---

## 📊 Admin Dashboard

### دریافت آمار داشبورد

```bash
curl -X GET "http://localhost:5000/api/admin/dashboard" \
  -H "Authorization: Bearer {accessToken}"
```

**پاسخ:**
```json
{
  "totalUsers": 150,
  "activeUsers": 120,
  "totalProducts": 5000,
  "trackedProducts": 3500,
  "totalScrapingJobs": 1000,
  "successfulJobs": 950,
  "failedJobs": 50,
  "notificationsSent": 10000,
  "averageResponseTime": 2.5,
  "userGrowth": [
    {
      "label": "2025-01-01",
      "value": 5
    }
  ],
  "priceChanges": [
    {
      "label": "2025-01-01",
      "value": 100
    }
  ]
}
```

### لیست کاربران

```bash
curl -X GET "http://localhost:5000/api/admin/users?page=1&pageSize=20" \
  -H "Authorization: Bearer {accessToken}"
```

### جزئیات کاربر

```bash
curl -X GET "http://localhost:5000/api/admin/users/{userId}" \
  -H "Authorization: Bearer {accessToken}"
```

### محصولات محبوب

```bash
curl -X GET "http://localhost:5000/api/admin/products/top?count=10" \
  -H "Authorization: Bearer {accessToken}"
```

### وضعیت سیستم

```bash
curl -X GET "http://localhost:5000/api/admin/health" \
  -H "Authorization: Bearer {accessToken}"
```

---

## 👤 مدیریت کاربران

### غیرفعال کردن کاربر

```bash
curl -X POST "http://localhost:5000/api/admin/users/{userId}/deactivate" \
  -H "Authorization: Bearer {accessToken}"
```

### فعال کردن مجدد کاربر

```bash
curl -X POST "http://localhost:5000/api/admin/users/{userId}/reactivate" \
  -H "Authorization: Bearer {accessToken}"
```

---

## 🔑 مدیریت ادمین

### ثبت ادمین جدید (SuperAdmin only)

```bash
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Authorization: Bearer {accessToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "newadmin",
    "password": "SecurePassword123!",
    "email": "newadmin@example.com",
    "fullName": "New Admin",
    "role": "Admin"
  }'
```

### تغییر رمز عبور

```bash
curl -X POST "http://localhost:5000/api/auth/change-password" \
  -H "Authorization: Bearer {accessToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "oldPassword": "currentPassword123!",
    "newPassword": "newPassword123!",
    "confirmPassword": "newPassword123!"
  }'
```

---

## 🔧 تنظیمات

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=smartprice;Username=postgres;Password=password"
  },
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "SmartPrice",
    "Audience": "SmartPriceUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

---

## 🛡️ نقش‌های ادمین

| نقش | دسترسی | توصیف |
|-----|--------|--------|
| SuperAdmin | تمام | دسترسی کامل به سیستم |
| Admin | اکثر | مدیریت کاربران و محصولات |
| Moderator | محدود | بررسی و تعدیل محتوا |
| Viewer | فقط مشاهده | دسترسی فقط به صفحات خواندنی |

---

## 📈 نظارت بر سیستم

### Metrics

سیستم به طور خودکار متریک‌های زیر را ثبت می‌کند:
- CPU Usage
- Memory Usage
- Active Jobs
- Database Performance
- Response Time

---

## 🐛 Troubleshooting

### "Invalid JWT Token"
- Access Token منقضی شده است
- **حل:** از endpoint refresh استفاده کنید

### "Unauthorized"
- Token ارسال نشده است یا نادرست است
- **حل:** اطمینان از صحیح بودن Authorization header

### "Forbidden"
- کاربر این دسترسی را ندارد
- **حل:** نقش کاربر را بررسی کنید

---

## 🚀 بعدی

1. Rate Limiting پیاده‌سازی
2. Logging و Monitoring بهتر
3. Docker Deployment
4. CI/CD Pipeline

---

**نسخه:** 1.0.0
**تاریخ:** 2025-01-01
**وضعیت:** ✅ آماده برای استفاده
