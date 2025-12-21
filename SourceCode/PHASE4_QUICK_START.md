# 🚀 Phase 4 Quick Start Guide - Telegram Bot

## Prerequisites
- Phases 1, 2, and 3 completed
- PostgreSQL running
- .NET 7 SDK installed
- Telegram account

## Step 1: Create Telegram Bot

1. **Open Telegram** and search for `@BotFather`
2. **Send Command**: `/newbot`
3. **Choose Name**: `SmartPrice Bot` (or your choice)
4. **Choose Username**: `SmartPriceBot` (must end with 'bot')
5. **Save Token**: Copy the bot token (looks like `123456789:ABCdefGHIjklMNOpqrsTUVwxyz`)

Example conversation:
```
You: /newbot
BotFather: Alright, a new bot. How are we going to call it?
You: SmartPrice Bot
BotFather: Good. Now let's choose a username for your bot...
You: SmartPriceBot
BotFather: Done! Your bot token is:
123456789:ABCdefGHIjklMNOpqrsTUVwxyz
```

## Step 2: Configure Bot Token

Edit `appsettings.json`:

```json
{
  "Telegram": {
    "BotToken": "123456789:ABCdefGHIjklMNOpqrsTUVwxyz",
    "ChannelId": "@your_channel"
  }
}
```

Replace with your actual bot token!

## Step 3: Apply Database Migration

```powershell
cd "D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode\src\SmartPrice.API"
dotnet ef database update --project ..\SmartPrice.Infrastructure\SmartPrice.Infrastructure.csproj
```

Expected output:
```
Applying migration '20251221020000_AddTelegramBotSupport'.
Done.
```

## Step 4: Start the Application

```powershell
cd "D:\Task\BackEnd\SmartPrice\Source\SmartPrice\SourceCode\src\SmartPrice.API"
dotnet run
```

Look for these log messages:
```
[INFO] Telegram Bot Background Service is starting
[INFO] Telegram bot started successfully: @SmartPriceBot (ID: 123456789)
[INFO] Bot is now listening for messages...
```

## Step 5: Test the Bot

1. **Open Telegram**
2. **Search** for your bot: `@SmartPriceBot`
3. **Click** "START" or send `/start`

You should receive:
```
👋 سلام! به SmartPrice خوش آمدید!

🤖 من یک ربات هوشمند برای رصد قیمت محصولات هستم.

با این ربات می‌تونید:
✅ قیمت محصولات دیجیکالا رو رصد کنید
✅ وقتی قیمت کاهش یافت، خبردار بشید
✅ محصولات ناموجود رو دنبال کنید
✅ قیمت هدف تعیین کنید

راهنما:
🔹 فقط لینک محصول دیجیکالا رو بفرستید
🔹 از دستور /help برای راهنمای کامل استفاده کنید
🔹 برای دیدن محصولات خود: /myproducts

برای شروع، لینک یک محصول از دیجیکالا رو ارسال کنید!
```

## Step 6: Track Your First Product

### Method 1: Using /track command

Send to bot:
```
/track https://www.digikala.com/product/dkp-12345678
```

### Method 2: Direct URL

Just send the URL:
```
https://www.digikala.com/product/dkp-12345678
```

**Bot Response**:
```
⏳ در حال بررسی محصول...

✅ محصول با موفقیت به لیست شما اضافه شد!

📬 به محض تغییر قیمت، به شما اطلاع می‌دهیم.

برای دیدن محصولات خود از دستور /myproducts استفاده کنید.
```

## Step 7: View Your Products

Send to bot:
```
/myproducts
```

Response example:
```
📦 محصولات من (2)

• گوشی موبایل سامسونگ Galaxy S23
  💰 قیمت فعلی: 45,000,000 تومان
  ✅ موجود
  📅 5 روز پیگیری
  📬 2 نوتیفیکیشن
  🔗 مشاهده

• هدفون بلوتوثی Sony WH-1000XM5
  💰 قیمت فعلی: 15,500,000 تومان
  🎯 قیمت هدف: 14,000,000 تومان
  ❌ ناموجود
  📅 3 روز پیگیری
  📬 1 نوتیفیکیشن
  🔗 مشاهده

برای حذف محصول از دستور /untrack استفاده کنید.
```

## Step 8: Get Help

Send to bot:
```
/help
```

Complete command reference will be displayed.

## Bot Commands Reference

| Command | Description | Example |
|---------|-------------|---------|
| `/start` | Start bot and see welcome | `/start` |
| `/help` | Show help and commands | `/help` |
| `/track` | Track a product | `/track https://digikala.com/...` |
| `/myproducts` | List your products | `/myproducts` |
| `/untrack` | Remove product | Coming soon |
| `/settings` | User settings | Coming soon |
| `/stats` | System stats (admin only) | `/stats` |
| `/cancel` | Cancel operation | `/cancel` |

## Testing Notifications

### Scenario 1: Price Drop

1. Track a product
2. Wait for scheduled job to scrape
3. If price drops, you'll receive:

```
📉 تغییر قیمت!

📦 گوشی موبایل سامسونگ Galaxy S23

💰 قیمت قبل: 45,000,000 تومان
💰 قیمت جدید: 43,000,000 تومان

📊 تغییر: 2,000,000 تومان (4.4%)

✅ موجود است

🔗 مشاهده محصول
```

### Scenario 2: Product Available

If an out-of-stock product becomes available:

```
✅ محصول موجود شد!

📦 هدفون بلوتوثی Sony WH-1000XM5

💰 قیمت: 15,500,000 تومان

🔗 مشاهده محصول
```

## Verify Database

Check if data is being saved:

```sql
-- Connect to PostgreSQL
psql -U postgres -d smartprice

-- Check Telegram users
SELECT "Id", "ChatId", "Username", "FirstName", "IsActive", "CreatedAt"
FROM "TelegramUsers"
ORDER BY "CreatedAt" DESC;

-- Check tracked products
SELECT 
    u."FirstName", 
    p."Name",
    t."TargetPrice",
    t."IsActive",
    t."CreatedAt"
FROM "UserProductTrackings" t
JOIN "TelegramUsers" u ON t."UserId" = u."Id"
JOIN "Products" p ON t."ProductId" = p."Id"
ORDER BY t."CreatedAt" DESC;

-- Check notifications
SELECT 
    u."FirstName",
    n."Type",
    n."IsSent",
    n."SentAt",
    n."Message"
FROM "NotificationLogs" n
JOIN "TelegramUsers" u ON n."UserId" = u."Id"
ORDER BY n."CreatedAt" DESC
LIMIT 10;
```

## Admin Features

### Set Admin Status

Update database to make a user admin:

```sql
UPDATE "TelegramUsers"
SET "IsAdmin" = true
WHERE "ChatId" = YOUR_CHAT_ID;
```

Find your chat ID from the logs or database.

### Use Admin Commands

Once admin, you can use:

```
/stats
```

Response:
```
📊 آمار سیستم

👥 کل کاربران: 15
✅ کاربران فعال: 12
📦 محصولات تحت رصد: 45
📬 نوتیفیکیشن‌های ارسالی: 123

⏰ آخرین بروزرسانی: 14:30
```

## Troubleshooting

### Issue: Bot not responding

**Check logs for**:
```
[ERROR] Failed to start Telegram bot
```

**Solutions**:
1. Verify bot token in appsettings.json
2. Check internet connection
3. Ensure token is correct and not revoked

### Issue: Bot starts but doesn't reply

**Check logs for**:
```
[INFO] Received message from {ChatId}
```

**If no logs**:
- Bot may be polling incorrectly
- Restart application
- Check firewall settings

### Issue: Tracking fails

**Check logs for**:
```
[ERROR] Failed to scrape product
```

**Solutions**:
1. Verify URL is from Digikala
2. Check scraper service is working
3. Test URL in browser

### Issue: No notifications received

**Check**:
1. User has `NotificationsEnabled = true`
2. Price actually changed
3. Rate limit (max 1 per hour)
4. Background job is running

## Configuration Tips

### Enable Debug Logging

In `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "SmartPrice.Infrastructure.Services.Telegram": "Debug"
      }
    }
  }
}
```

### Adjust Notification Rate Limit

In `NotificationService.cs`:
```csharp
// Change from 1 hour to 30 minutes
var thirtyMinutesAgo = DateTime.UtcNow.AddMinutes(-30);
```

## Example Usage Scenarios

### Scenario 1: Single Product Monitoring

```
User: /start
Bot: [Welcome message]

User: https://www.digikala.com/product/dkp-123456
Bot: ⏳ در حال بررسی محصول...
Bot: ✅ محصول با موفقیت به لیست شما اضافه شد!

[Later, when price drops]
Bot: 📉 تغییر قیمت! ...
```

### Scenario 2: Multiple Products

```
User: https://www.digikala.com/product/dkp-111111
Bot: ✅ محصول اضافه شد!

User: https://www.digikala.com/product/dkp-222222
Bot: ✅ محصول اضافه شد!

User: /myproducts
Bot: 📦 محصولات من (2) ...
```

### Scenario 3: Help Request

```
User: /help
Bot: [Complete command list]

User: کمک
Bot: ❓ دستور نامعتبر. از /help استفاده کنید.
```

## Success Indicators

Phase 4 is working correctly when:

1. ✅ Bot starts without errors
2. ✅ `/start` command works
3. ✅ Users registered in database
4. ✅ Products can be tracked
5. ✅ `/myproducts` shows tracked items
6. ✅ Notifications sent when prices change
7. ✅ Logs show bot activity
8. ✅ Database has user and tracking records

## Next Steps After Setup

1. **Share Bot**: Share your bot link with users
2. **Monitor Logs**: Watch for errors or issues
3. **Test Notifications**: Change prices manually to test
4. **Set Up Admin**: Make yourself admin for stats
5. **Document**: Keep track of your bot token securely

## Important Notes

- **Bot Token Security**: Never commit token to git
- **Database Backups**: Backup before testing
- **Rate Limiting**: Bot respects Telegram rate limits
- **Error Recovery**: Bot auto-recovers from most errors
- **Scaling**: Bot can handle thousands of users

---

**🎉 Your Telegram bot is now live and ready to help users track prices!**

Users can start chatting with your bot and getting real-time price alerts for their favorite products!
