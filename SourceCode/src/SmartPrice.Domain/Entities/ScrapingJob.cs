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
    /// The URL being scraped
    /// </summary>
    public string TargetUrl { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the job
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// When the job started
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// When the job completed (null if still running or failed)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Number of products successfully scraped
    /// </summary>
    public int ProductsScraped { get; set; }

    /// <summary>
    /// Error message if the job failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
