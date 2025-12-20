namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a Telegram channel for posting product updates
/// </summary>
public class TelegramChannel
{
    /// <summary>
    /// Unique identifier for the channel record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Telegram channel ID (e.g., @channelname or numeric ID)
    /// </summary>
    public string ChannelId { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable channel name
    /// </summary>
    public string ChannelName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this channel is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// When this channel was added to the system
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
