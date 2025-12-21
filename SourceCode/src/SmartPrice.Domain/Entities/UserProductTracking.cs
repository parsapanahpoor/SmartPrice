namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a user's tracking of a specific product
/// </summary>
public class UserProductTracking
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
    /// Reference to the product
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Target price for notifications (optional)
    /// </summary>
    public decimal? TargetPrice { get; set; }

    /// <summary>
    /// Notify on any price change
    /// </summary>
    public bool NotifyOnAnyPriceChange { get; set; }

    /// <summary>
    /// Notify when product becomes available
    /// </summary>
    public bool NotifyOnAvailability { get; set; } = true;

    /// <summary>
    /// Whether this tracking is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last time notification was sent for this tracking
    /// </summary>
    public DateTime? LastNotifiedAt { get; set; }

    /// <summary>
    /// Total number of notifications sent
    /// </summary>
    public int NotificationCount { get; set; }

    /// <summary>
    /// When the tracking was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the tracking was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// Navigation to the user
    /// </summary>
    public TelegramUser User { get; set; } = null!;

    /// <summary>
    /// Navigation to the product
    /// </summary>
    public Product Product { get; set; } = null!;
}
