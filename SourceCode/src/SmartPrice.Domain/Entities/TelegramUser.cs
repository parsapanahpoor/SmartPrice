namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a Telegram user in the system
/// </summary>
public class TelegramUser
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Telegram chat ID
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// Telegram username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether the user is an administrator
    /// </summary>
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// Whether notifications are enabled for this user
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Last time user interacted with the bot
    /// </summary>
    public DateTime? LastInteractionAt { get; set; }

    /// <summary>
    /// User's language code (e.g., 'fa', 'en')
    /// </summary>
    public string? LanguageCode { get; set; } = "fa";

    /// <summary>
    /// When the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the user was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// Products tracked by this user
    /// </summary>
    public ICollection<UserProductTracking> TrackedProducts { get; set; } = new List<UserProductTracking>();

    /// <summary>
    /// Notifications sent to this user
    /// </summary>
    public ICollection<NotificationLog> Notifications { get; set; } = new List<NotificationLog>();
}
