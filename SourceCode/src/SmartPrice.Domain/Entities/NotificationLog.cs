using SmartPrice.Domain.Enums;

namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a log of notifications sent to users
/// </summary>
public class NotificationLog
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Reference to the product (optional)
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Type of notification
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Notification message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether the notification was successfully sent
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// When the notification was sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// When the notification log was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the notification log was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// Navigation to the user
    /// </summary>
    public TelegramUser User { get; set; } = null!;

    /// <summary>
    /// Navigation to the product (optional)
    /// </summary>
    public Product? Product { get; set; }
}
