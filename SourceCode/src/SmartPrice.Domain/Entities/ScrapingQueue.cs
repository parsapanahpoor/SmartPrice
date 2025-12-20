using SmartPrice.Domain.Enums;

namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a queue item for scraping operations
/// </summary>
public class ScrapingQueue
{
    /// <summary>
    /// Unique identifier for the queue item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Reference to the parent scraping job
    /// </summary>
    public Guid ScrapingJobId { get; set; }

    /// <summary>
    /// URL to be scraped
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Type of marketplace
    /// </summary>
    public MarketplaceType Marketplace { get; set; }

    /// <summary>
    /// Priority level of this queue item
    /// </summary>
    public JobPriority Priority { get; set; }

    /// <summary>
    /// Current status of the queue item
    /// </summary>
    public ScrapingStatus Status { get; set; }

    /// <summary>
    /// Number of retry attempts made
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// When this item was scheduled
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// When this item was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// JSON result of scraping operation
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Error message if processing failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the queue item was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the queue item was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// Navigation property to the parent scraping job
    /// </summary>
    public ScrapingJob ScrapingJob { get; set; } = null!;
}
