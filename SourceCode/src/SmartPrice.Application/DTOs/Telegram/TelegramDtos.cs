namespace SmartPrice.Application.DTOs.Telegram;

/// <summary>
/// DTO for user product tracking information
/// </summary>
public class UserProductTrackingDto
{
    /// <summary>
    /// Tracking identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Product title
    /// </summary>
    public string ProductTitle { get; set; } = string.Empty;

    /// <summary>
    /// Product URL
    /// </summary>
    public string ProductUrl { get; set; } = string.Empty;

    /// <summary>
    /// Current product price
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// User's target price (optional)
    /// </summary>
    public decimal? TargetPrice { get; set; }

    /// <summary>
    /// Whether product is available
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// When tracking started
    /// </summary>
    public DateTime TrackedSince { get; set; }

    /// <summary>
    /// Last notification time
    /// </summary>
    public DateTime? LastNotifiedAt { get; set; }

    /// <summary>
    /// Product image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Total notifications sent
    /// </summary>
    public int NotificationCount { get; set; }
}

/// <summary>
/// DTO for Telegram message
/// </summary>
public class TelegramMessageDto
{
    /// <summary>
    /// Chat identifier
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// Message text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Username (optional)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// First name (optional)
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name (optional)
    /// </summary>
    public string? LastName { get; set; }
}

/// <summary>
/// DTO for user statistics
/// </summary>
public class UserStatsDto
{
    /// <summary>
    /// Total users
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Active users
    /// </summary>
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Total tracked products
    /// </summary>
    public int TotalTrackedProducts { get; set; }

    /// <summary>
    /// Total notifications sent
    /// </summary>
    public int TotalNotificationsSent { get; set; }
}
