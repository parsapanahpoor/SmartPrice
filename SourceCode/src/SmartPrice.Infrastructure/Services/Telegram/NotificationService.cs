using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces;
using SmartPrice.Application.Interfaces.Telegram;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Services.Telegram;

/// <summary>
/// Service for managing notifications
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ITelegramBotService _botService;
    private readonly IRepository<NotificationLog> _notificationRepository;
    private readonly IRepository<TelegramUser> _userRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ITelegramBotService botService,
        IRepository<NotificationLog> notificationRepository,
        IRepository<TelegramUser> userRepository,
        ILogger<NotificationService> logger)
    {
        _botService = botService;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task SendPriceAlertAsync(UserProductTracking tracking, decimal oldPrice, decimal newPrice, CancellationToken ct)
    {
        if (!await CanSendNotificationAsync(tracking.Id, ct))
        {
            _logger.LogDebug("Rate limit: Cannot send notification for tracking {TrackingId}", tracking.Id);
            return;
        }

        var type = newPrice < oldPrice ? NotificationType.PriceDropped : NotificationType.PriceIncreased;
        var priceChange = newPrice - oldPrice;
        var percentageChange = (priceChange / oldPrice) * 100;

        var emoji = newPrice < oldPrice ? "📉" : "📈";
        var changeText = newPrice < oldPrice ? "کاهش یافت" : "افزایش یافت";

        var message = $@"{emoji} <b>تغییر قیمت!</b>

📦 <b>{tracking.Product.Name}</b>

💰 قیمت قبل: <s>{oldPrice:N0}</s> تومان
💰 قیمت جدید: <b>{newPrice:N0}</b> تومان

📊 تغییر: {Math.Abs(priceChange):N0} تومان ({Math.Abs(percentageChange):F1}%)

{(tracking.Product.IsAvailable ? "✅ موجود است" : "❌ ناموجود")}

🔗 <a href=""{tracking.Product.Url}"">مشاهده محصول</a>";

        await SendAndLogNotificationAsync(tracking.UserId, tracking.ProductId, type, message, ct);

        // Update tracking
        tracking.LastNotifiedAt = DateTime.UtcNow;
        tracking.NotificationCount++;
    }

    public async Task SendAvailabilityAlertAsync(UserProductTracking tracking, bool isAvailable, CancellationToken ct)
    {
        if (!tracking.NotifyOnAvailability)
        {
            return;
        }

        var message = isAvailable
            ? $@"✅ <b>محصول موجود شد!</b>

📦 <b>{tracking.Product.Name}</b>

💰 قیمت: <b>{tracking.Product.CurrentPrice:N0}</b> تومان

🔗 <a href=""{tracking.Product.Url}"">مشاهده محصول</a>"
            : $@"❌ <b>محصول ناموجود شد</b>

📦 <b>{tracking.Product.Name}</b>

🔔 به محض موجود شدن به شما اطلاع می‌دهیم.";

        await SendAndLogNotificationAsync(tracking.UserId, tracking.ProductId, NotificationType.AvailabilityChanged, message, ct);

        tracking.LastNotifiedAt = DateTime.UtcNow;
        tracking.NotificationCount++;
    }

    public async Task SendWelcomeMessageAsync(long chatId, CancellationToken ct)
    {
        var welcomeMessage = @"👋 <b>سلام! به SmartPrice خوش آمدید!</b>

🤖 من یک ربات هوشمند برای رصد قیمت محصولات هستم.

<b>با این ربات می‌تونید:</b>
✅ قیمت محصولات دیجیکالا رو رصد کنید
✅ وقتی قیمت کاهش یافت، خبردار بشید
✅ محصولات ناموجود رو دنبال کنید
✅ قیمت هدف تعیین کنید

<b>راهنما:</b>
🔹 فقط لینک محصول دیجیکالا رو بفرستید
🔹 از دستور /help برای راهنمای کامل استفاده کنید
🔹 برای دیدن محصولات خود: /myproducts

<i>برای شروع، لینک یک محصول از دیجیکالا رو ارسال کنید!</i>";

        await _botService.SendMessageAsync(chatId, welcomeMessage, ct);
    }

    public async Task SendDailyReportAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null || !user.NotificationsEnabled)
        {
            return;
        }

        // Implementation for daily report
        // This can be expanded to show price changes over the day
        var reportMessage = @"📊 <b>گزارش روزانه</b>

این قابلیت به زودی فعال می‌شود.";

        await _botService.SendMessageAsync(user.ChatId, reportMessage, ct);
    }

    public async Task<bool> CanSendNotificationAsync(Guid trackingId, CancellationToken ct)
    {
        // Rate limiting: max 1 notification per hour per tracking
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);

        var recentNotifications = await _notificationRepository.FindAsync(
            n => n.UserId == trackingId && n.SentAt >= oneHourAgo && n.IsSent,
            ct);

        return recentNotifications.Count == 0;
    }

    public async Task SendTargetPriceReachedAsync(UserProductTracking tracking, CancellationToken ct)
    {
        if (tracking.TargetPrice == null)
        {
            return;
        }

        var message = $@"🎯 <b>به قیمت هدف رسید!</b>

📦 <b>{tracking.Product.Name}</b>

💰 قیمت فعلی: <b>{tracking.Product.CurrentPrice:N0}</b> تومان
🎯 قیمت هدف: {tracking.TargetPrice.Value:N0} تومان

{(tracking.Product.IsAvailable ? "✅ موجود است" : "❌ ناموجود")}

🔗 <a href=""{tracking.Product.Url}"">مشاهده محصول</a>";

        await SendAndLogNotificationAsync(tracking.UserId, tracking.ProductId, NotificationType.TargetPriceReached, message, ct);

        tracking.LastNotifiedAt = DateTime.UtcNow;
        tracking.NotificationCount++;
    }

    private async Task SendAndLogNotificationAsync(
        Guid userId,
        Guid productId,
        NotificationType type,
        string message,
        CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null || !user.NotificationsEnabled)
        {
            _logger.LogDebug("User {UserId} has notifications disabled", userId);
            return;
        }

        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
            Type = type,
            Message = message,
            IsSent = false,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            await _botService.SendMessageAsync(user.ChatId, message, ct);

            log.IsSent = true;
            log.SentAt = DateTime.UtcNow;

            _logger.LogInformation("Notification sent to user {UserId}: Type={Type}",
                userId, type);
        }
        catch (Exception ex)
        {
            log.IsSent = false;
            log.ErrorMessage = ex.Message;
            log.RetryCount++;

            _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
        }

        await _notificationRepository.AddAsync(log, ct);
    }
}
