using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces.Telegram;

/// <summary>
/// Service for managing notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Send a price alert notification
    /// </summary>
    /// <param name="tracking">Product tracking record</param>
    /// <param name="oldPrice">Previous price</param>
    /// <param name="newPrice">New price</param>
    /// <param name="ct">Cancellation token</param>
    Task SendPriceAlertAsync(UserProductTracking tracking, decimal oldPrice, decimal newPrice, CancellationToken ct);

    /// <summary>
    /// Send an availability alert notification
    /// </summary>
    /// <param name="tracking">Product tracking record</param>
    /// <param name="isAvailable">Whether product is now available</param>
    /// <param name="ct">Cancellation token</param>
    Task SendAvailabilityAlertAsync(UserProductTracking tracking, bool isAvailable, CancellationToken ct);

    /// <summary>
    /// Send welcome message to new user
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="ct">Cancellation token</param>
    Task SendWelcomeMessageAsync(long chatId, CancellationToken ct);

    /// <summary>
    /// Send daily report to user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="ct">Cancellation token</param>
    Task SendDailyReportAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Check if a notification can be sent (rate limiting)
    /// </summary>
    /// <param name="trackingId">Tracking identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if notification can be sent</returns>
    Task<bool> CanSendNotificationAsync(Guid trackingId, CancellationToken ct);

    /// <summary>
    /// Send target price reached notification
    /// </summary>
    /// <param name="tracking">Product tracking record</param>
    /// <param name="ct">Cancellation token</param>
    Task SendTargetPriceReachedAsync(UserProductTracking tracking, CancellationToken ct);
}
