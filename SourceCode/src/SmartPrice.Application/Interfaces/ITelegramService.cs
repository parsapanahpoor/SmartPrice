using SmartPrice.Application.DTOs;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Interface for Telegram bot operations
/// </summary>
public interface ITelegramService
{
    /// <summary>
    /// Sends a single product message to the specified channel
    /// </summary>
    /// <param name="channelId">Telegram channel ID</param>
    /// <param name="product">Product to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendProductMessageAsync(
        string channelId,
        ProductDto product,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends multiple products to the specified channel
    /// </summary>
    /// <param name="channelId">Telegram channel ID</param>
    /// <param name="products">Products to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendBulkProductsAsync(
        string channelId,
        IEnumerable<ProductDto> products,
        CancellationToken cancellationToken = default
    );
}
