using SmartPrice.Domain.Enums;

namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a scraping job execution
/// </summary>
public class ScrapingJob
{
    /// <summary>
    /// Unique identifier for the job
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Job name for identification
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The URL being scraped
    /// </summary>
    public string TargetUrl { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the job
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// Type of marketplace being scraped
    /// </summary>
    public MarketplaceType Marketplace { get; set; }

    /// <summary>
    /// Number of retry attempts made
    /// </summary>
    public int RetryCount { get; set; }

    // Scheduling Properties
    /// <summary>
    /// How frequently the job should run
    /// </summary>
    public JobFrequency Frequency { get; set; }

    /// <summary>
    /// Cron expression for custom scheduling
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// Next scheduled run time
    /// </summary>
    public DateTime? NextRunAt { get; set; }

    /// <summary>
    /// Last time the job was executed
    /// </summary>
    public DateTime? LastRunAt { get; set; }

    /// <summary>
    /// Total number of times the job has been executed
    /// </summary>
    public int RunCount { get; set; }

    /// <summary>
    /// Priority level of the job
    /// </summary>
    public JobPriority Priority { get; set; }

    // Configuration Properties
    /// <summary>
    /// Whether the job is active and should be scheduled
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Timeout for job execution
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    // Results Properties
    /// <summary>
    /// Number of successful executions
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed executions
    /// </summary>
    public int FailureCount { get; set; }

    // Existing Properties
    /// <summary>
    /// When the job started
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// When the job completed (null if still running or failed)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Duration of the scraping job
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Number of products successfully scraped
    /// </summary>
    public int ProductsScraped { get; set; }

    /// <summary>
    /// Error message if the job failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the job was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the job was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
