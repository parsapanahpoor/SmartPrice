using Microsoft.Extensions.Logging;
using SmartPrice.Application.DTOs.Telegram;
using SmartPrice.Application.Interfaces.Telegram;

namespace SmartPrice.Infrastructure.Services.Telegram;

/// <summary>
/// Handles Telegram bot commands
/// </summary>
public class CommandHandler : ICommandHandler
{
    private readonly ITelegramBotService _botService;
    private readonly IUserService _userService;
    private readonly ITrackingService _trackingService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(
        ITelegramBotService botService,
        IUserService userService,
        ITrackingService trackingService,
        INotificationService notificationService,
        ILogger<CommandHandler> logger)
    {
        _botService = botService;
        _userService = userService;
        _trackingService = trackingService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleCommandAsync(TelegramMessageDto message, CancellationToken ct)
    {
        var chatId = message.ChatId;
        var text = message.Text ?? string.Empty;

        try
        {
            // Get or create user
            var user = await _userService.GetOrCreateUserAsync(
                chatId,
                message.Username,
                message.FirstName,
                ct);

            // Update last interaction
            await _userService.UpdateUserInteractionAsync(chatId, ct);

            // Parse and handle command
            var command = ParseCommand(text);

            await (command switch
            {
                "/start" => HandleStartAsync(chatId, ct),
                "/help" => HandleHelpAsync(chatId, ct),
                "/track" => HandleTrackAsync(chatId, text, user.Id, ct),
                "/untrack" => HandleUntrackAsync(chatId, user.Id, ct),
                "/myproducts" => HandleMyProductsAsync(chatId, user.Id, ct),
                "/settings" => HandleSettingsAsync(chatId, user.Id, ct),
                "/stats" when await _userService.IsUserAdminAsync(chatId, ct) => HandleStatsAsync(chatId, ct),
                "/cancel" => HandleCancelAsync(chatId, ct),
                _ when IsUrl(text) => HandleUrlAsync(chatId, text, user.Id, ct),
                _ => HandleUnknownAsync(chatId, ct)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling command from {ChatId}: {Command}", chatId, text);
            await _botService.SendMessageAsync(
                chatId,
                "❌ خطایی رخ داد. لطفاً دوباره تلاش کنید یا از /help استفاده کنید.",
                ct);
        }
    }

    private async Task HandleStartAsync(long chatId, CancellationToken ct)
    {
        await _notificationService.SendWelcomeMessageAsync(chatId, ct);
    }

    private async Task HandleHelpAsync(long chatId, CancellationToken ct)
    {
        var helpText = @"📚 <b>راهنمای استفاده از ربات</b>

<b>دستورات اصلی:</b>

🔸 <b>/track [لینک]</b> - دنبال کردن محصول
   مثال: /track https://digikala.com/product/...

🔸 <b>/myproducts</b> - لیست محصولات من
   نمایش تمام محصولاتی که دنبال می‌کنید

🔸 <b>/untrack</b> - حذف محصول از لیست
   (به زودی فعال می‌شود)

🔸 <b>/settings</b> - تنظیمات شخصی
   تنظیمات نوتیفیکیشن و ...

🔸 <b>/help</b> - نمایش این راهنما

🔸 <b>/cancel</b> - لغو عملیات فعلی

<b>نکات مهم:</b>
✅ می‌تونید فقط لینک محصول رو بفرستید
✅ پشتیبانی از محصولات دیجیکالا
✅ نوتیفیکیشن رایگان برای تغییر قیمت
✅ رصد موجودی محصولات

<i>سوال دارید؟ فقط بپرسید!</i>";

        await _botService.SendMessageAsync(chatId, helpText, ct);
    }

    private async Task HandleTrackAsync(long chatId, string text, Guid userId, CancellationToken ct)
    {
        var url = ExtractUrl(text);
        if (string.IsNullOrEmpty(url))
        {
            await _botService.SendMessageAsync(
                chatId,
                "❌ لطفاً لینک محصول را وارد کنید.\n\nمثال:\n/track https://digikala.com/product/dkp-123456",
                ct);
            return;
        }

        // Check if already tracking
        if (await _trackingService.IsTrackingProductAsync(userId, url, ct))
        {
            await _botService.SendMessageAsync(
                chatId,
                "ℹ️ شما از قبل این محصول را دنبال می‌کنید.",
                ct);
            return;
        }

        // Send processing message
        await _botService.SendMessageAsync(
            chatId,
            "⏳ در حال بررسی محصول...",
            ct);

        try
        {
            var trackingId = await _trackingService.TrackProductAsync(userId, url, null, ct);

            await _botService.SendMessageAsync(
                chatId,
                "✅ محصول با موفقیت به لیست شما اضافه شد!\n\n" +
                "📬 به محض تغییر قیمت، به شما اطلاع می‌دهیم.\n\n" +
                "برای دیدن محصولات خود از دستور /myproducts استفاده کنید.",
                ct);

            _logger.LogInformation("User {UserId} started tracking product: {Url}", userId, url);
        }
        catch (ArgumentException ex)
        {
            await _botService.SendMessageAsync(
                chatId,
                $"❌ لینک نامعتبر است.\n\n{ex.Message}\n\nلطفاً لینک دیجیکالا را ارسال کنید.",
                ct);
        }
        catch (InvalidOperationException ex)
        {
            await _botService.SendMessageAsync(
                chatId,
                $"❌ خطا در دریافت اطلاعات محصول.\n\n{ex.Message}\n\nلطفاً دوباره تلاش کنید.",
                ct);
        }
    }

    private async Task HandleMyProductsAsync(long chatId, Guid userId, CancellationToken ct)
    {
        var products = await _trackingService.GetUserTrackedProductsAsync(userId, ct);

        if (!products.Any())
        {
            await _botService.SendMessageAsync(
                chatId,
                "📦 شما هیچ محصولی را دنبال نمی‌کنید.\n\n" +
                "برای شروع، لینک یک محصول از دیجیکالا را ارسال کنید!",
                ct);
            return;
        }

        var message = $"📦 <b>محصولات من ({products.Count})</b>\n\n";

        foreach (var product in products)
        {
            var trackedDays = (DateTime.UtcNow - product.TrackedSince).Days;

            message += $"• <b>{product.ProductTitle}</b>\n";
            message += $"  💰 قیمت فعلی: <b>{product.CurrentPrice:N0}</b> تومان\n";

            if (product.TargetPrice.HasValue)
            {
                message += $"  🎯 قیمت هدف: {product.TargetPrice.Value:N0} تومان\n";
            }

            message += $"  {(product.IsAvailable ? "✅ موجود" : "❌ ناموجود")}\n";
            message += $"  📅 {trackedDays} روز پیگیری\n";
            message += $"  📬 {product.NotificationCount} نوتیفیکیشن\n";
            message += $"  🔗 <a href=\"{product.ProductUrl}\">مشاهده</a>\n\n";
        }

        message += "<i>برای حذف محصول از دستور /untrack استفاده کنید.</i>";

        await _botService.SendMessageAsync(chatId, message, ct);
    }

    private async Task HandleUntrackAsync(long chatId, Guid userId, CancellationToken ct)
    {
        await _botService.SendMessageAsync(
            chatId,
            "🔧 این قابلیت به زودی اضافه می‌شود.\n\n" +
            "در حال حاضر از /myproducts برای دیدن محصولات خود استفاده کنید.",
            ct);
    }

    private async Task HandleSettingsAsync(long chatId, Guid userId, CancellationToken ct)
    {
        await _botService.SendMessageAsync(
            chatId,
            "⚙️ <b>تنظیمات</b>\n\n" +
            "این بخش به زودی با امکانات زیر فعال می‌شود:\n\n" +
            "• فعال/غیرفعال کردن نوتیفیکیشن‌ها\n" +
            "• تنظیم قیمت هدف\n" +
            "• انتخاب زبان\n" +
            "• گزارش روزانه",
            ct);
    }

    private async Task HandleStatsAsync(long chatId, CancellationToken ct)
    {
        var totalUsers = await _userService.GetTotalUsersCountAsync(ct);
        var activeUsers = await _userService.GetActiveUsersCountAsync(ct);

        var statsMessage = $@"📊 <b>آمار سیستم</b>

👥 کل کاربران: {totalUsers}
✅ کاربران فعال: {activeUsers}
📦 محصولات تحت رصد: -
📬 نوتیفیکیشن‌های ارسالی: -

⏰ آخرین بروزرسانی: {DateTime.Now:HH:mm}";

        await _botService.SendMessageAsync(chatId, statsMessage, ct);
    }

    private async Task HandleUrlAsync(long chatId, string url, Guid userId, CancellationToken ct)
    {
        // Validate it's a Digikala URL
        if (!url.Contains("digikala.com", StringComparison.OrdinalIgnoreCase))
        {
            await _botService.SendMessageAsync(
                chatId,
                "❌ فقط لینک‌های دیجیکالا پشتیبانی می‌شوند.\n\n" +
                "مثال:\nhttps://www.digikala.com/product/dkp-123456",
                ct);
            return;
        }

        // Treat as track command
        await HandleTrackAsync(chatId, url, userId, ct);
    }

    private async Task HandleCancelAsync(long chatId, CancellationToken ct)
    {
        await _botService.SendMessageAsync(
            chatId,
            "✅ عملیات لغو شد.\n\nاز /help برای راهنما استفاده کنید.",
            ct);
    }

    private async Task HandleUnknownAsync(long chatId, CancellationToken ct)
    {
        await _botService.SendMessageAsync(
            chatId,
            "❓ دستور نامعتبر.\n\nاز /help برای مشاهده دستورات استفاده کنید.",
            ct);
    }

    private string ParseCommand(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0].ToLowerInvariant() : string.Empty;
    }

    private string? ExtractUrl(string text)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.FirstOrDefault(p => p.StartsWith("http", StringComparison.OrdinalIgnoreCase));
    }

    private bool IsUrl(string text)
    {
        return text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
               text.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }
}
