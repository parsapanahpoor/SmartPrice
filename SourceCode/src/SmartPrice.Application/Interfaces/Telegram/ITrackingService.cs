using SmartPrice.Application.DTOs.Telegram;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Application.Interfaces.Telegram;

/// <summary>
/// Service for managing product tracking
/// </summary>
public interface ITrackingService
{
    /// <summary>
    /// Start tracking a product for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productUrl">Product URL</param>
    /// <param name="targetPrice">Optional target price</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Tracking identifier</returns>
    Task<Guid> TrackProductAsync(Guid userId, string productUrl, decimal? targetPrice, CancellationToken ct);

    /// <summary>
    /// Stop tracking a product
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if successful</returns>
    Task<bool> UntrackProductAsync(Guid userId, Guid productId, CancellationToken ct);

    /// <summary>
    /// Get all products tracked by a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of tracked products</returns>
    Task<List<UserProductTrackingDto>> GetUserTrackedProductsAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Update the target price for a tracking
    /// </summary>
    /// <param name="trackingId">Tracking identifier</param>
    /// <param name="targetPrice">New target price</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateTargetPriceAsync(Guid trackingId, decimal targetPrice, CancellationToken ct);

    /// <summary>
    /// Get all users who should be notified about a product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of tracking records</returns>
    Task<List<UserProductTracking>> GetUsersToNotifyAsync(Guid productId, CancellationToken ct);

    /// <summary>
    /// Check if a user is already tracking a product
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="productUrl">Product URL</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if already tracking</returns>
    Task<bool> IsTrackingProductAsync(Guid userId, string productUrl, CancellationToken ct);
}
