using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces;
using SmartPrice.Application.Interfaces.Telegram;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SmartPrice.Infrastructure.Services.Telegram;

/// <summary>
/// Main Telegram bot service
/// </summary>
public class TelegramBotService : ITelegramBotService, IDisposable
{
    private readonly TelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly string _botToken;
    private CancellationTokenSource? _cts;

    public TelegramBotService(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<TelegramBotService> logger)
    {
        _botToken = configuration["Telegram:BotToken"]
            ?? throw new InvalidOperationException("Telegram Bot Token not configured in appsettings.json");

        _botClient = new TelegramBotClient(_botToken);
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            var me = await _botClient.GetMeAsync(ct);
            _logger.LogInformation("Telegram bot started successfully: @{Username} (ID: {Id})",
                me.Username, me.Id);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message },
                ThrowPendingUpdates = true
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );

            _logger.LogInformation("Bot is now listening for messages...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Telegram bot");
            throw;
        }
    }

    public Task StopAsync(CancellationToken ct)
    {
        _cts?.Cancel();
        _logger.LogInformation("Telegram bot stopped");
        return Task.CompletedTask;
    }

    public async Task SendMessageAsync(long chatId, string message, CancellationToken ct)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true,
                cancellationToken: ct
            );

            _logger.LogDebug("Message sent to chat {ChatId}", chatId);
        }
        catch (ApiRequestException ex) when (ex.ErrorCode == 403)
        {
            _logger.LogWarning("Bot was blocked by user {ChatId}", chatId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to chat {ChatId}", chatId);
            throw;
        }
    }

    public async Task SendProductNotificationAsync(Guid userId, Guid productId, NotificationType type, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<TelegramUser>>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user == null || !user.NotificationsEnabled)
        {
            return;
        }

        var product = await productRepository.GetByIdAsync(productId, ct);
        if (product == null)
        {
            return;
        }

        var message = FormatNotificationMessage(product, type);
        await SendMessageAsync(user.ChatId, message, ct);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        try
        {
            if (update.Message is not { Text: { } messageText } message)
            {
                return;
            }

            var chatId = message.Chat.Id;
            var username = message.From?.Username;

            _logger.LogInformation("Received message from {ChatId} (@{Username}): {Text}",
                chatId, username, messageText);

            using var scope = _serviceProvider.CreateScope();
            var commandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler>();

            // Convert to our DTO
            var messageDto = new Application.DTOs.Telegram.TelegramMessageDto
            {
                ChatId = chatId,
                Text = messageText,
                Username = message.From?.Username,
                FirstName = message.From?.FirstName,
                LastName = message.From?.LastName
            };

            await commandHandler.HandleCommandAsync(messageDto, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling update: {UpdateId}", update.Id);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiEx => $"Telegram API Error [{apiEx.ErrorCode}]: {apiEx.Message}",
            _ => exception.Message
        };

        _logger.LogError(exception, "Bot error occurred: {ErrorMessage}", errorMessage);

        return Task.CompletedTask;
    }

    private string FormatNotificationMessage(Product product, NotificationType type)
    {
        return type switch
        {
            NotificationType.PriceDropped =>
                $@"📉 <b>قیمت کاهش یافت!</b>

📦 <b>{product.Name}</b>

💰 قیمت جدید: <b>{product.CurrentPrice:N0}</b> تومان

{(product.IsAvailable ? "✅ موجود است" : "❌ ناموجود")}

🔗 <a href=""{product.Url}"">مشاهده محصول</a>",

            NotificationType.PriceIncreased =>
                $@"📈 <b>قیمت افزایش یافت</b>

📦 <b>{product.Name}</b>

💰 قیمت جدید: <b>{product.CurrentPrice:N0}</b> تومان

{(product.IsAvailable ? "✅ موجود است" : "❌ ناموجود")}

🔗 <a href=""{product.Url}"">مشاهده محصول</a>",

            NotificationType.TargetPriceReached =>
                $@"🎯 <b>به قیمت هدف رسید!</b>

📦 <b>{product.Name}</b>

💰 قیمت: <b>{product.CurrentPrice:N0}</b> تومان

✅ موجود است

🔗 <a href=""{product.Url}"">مشاهده محصول</a>",

            NotificationType.AvailabilityChanged =>
                $@"✅ <b>محصول موجود شد!</b>

📦 <b>{product.Name}</b>

💰 قیمت: <b>{product.CurrentPrice:N0}</b> تومان

🔗 <a href=""{product.Url}"">مشاهده محصول</a>",

            _ => $@"📢 <b>{product.Name}</b>

💰 {product.CurrentPrice:N0} تومان

🔗 <a href=""{product.Url}"">مشاهده محصول</a>"
        };
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
