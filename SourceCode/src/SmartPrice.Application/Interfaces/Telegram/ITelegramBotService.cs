using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces.Telegram;

/// <summary>
/// Service for managing Telegram bot operations
/// </summary>
public interface ITelegramBotService
{
    /// <summary>
    /// Start the Telegram bot
    /// </summary>
    Task StartAsync(CancellationToken ct);

    /// <summary>
    /// Stop the Telegram bot
    /// </summary>
    Task StopAsync(CancellationToken ct);

    /// <summary>
    /// Send a text message to a user
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="message">Message content</param>
    /// <param name="ct">Cancellation token</param>
    Task SendMessageAsync(long chatId, string message, CancellationToken ct);

    /// <summary>
    /// Send a product notification to a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="type">Notification type</param>
    /// <param name="ct">Cancellation token</param>
    Task SendProductNotificationAsync(Guid userId, Guid productId, NotificationType type, CancellationToken ct);
}
