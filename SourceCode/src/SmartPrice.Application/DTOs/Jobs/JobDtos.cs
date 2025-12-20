using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.DTOs.Jobs;

/// <summary>
/// DTO for creating a new scraping job
/// </summary>
public class CreateJobDto
{
    /// <summary>
    /// Job name for identification
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// List of URLs to scrape
    /// </summary>
    public List<string> Urls { get; set; } = new();

    /// <summary>
    /// How frequently the job should run
    /// </summary>
    public JobFrequency Frequency { get; set; }

    /// <summary>
    /// Cron expression for custom scheduling
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// Priority level of the job
    /// </summary>
    public JobPriority Priority { get; set; } = JobPriority.Normal;

    /// <summary>
    /// Whether the job is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}

/// <summary>
/// DTO for job status information
/// </summary>
public class JobStatusDto
{
    /// <summary>
    /// Job identifier
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Job name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current job status
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// Next scheduled run time
    /// </summary>
    public DateTime? NextRunAt { get; set; }

    /// <summary>
    /// Last execution time
    /// </summary>
    public DateTime? LastRunAt { get; set; }

    /// <summary>
    /// Current queue length
    /// </summary>
    public int QueueLength { get; set; }

    /// <summary>
    /// Total number of runs
    /// </summary>
    public int TotalRuns { get; set; }

    /// <summary>
    /// Number of successful runs
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed runs
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Job frequency
    /// </summary>
    public JobFrequency Frequency { get; set; }

    /// <summary>
    /// Job priority
    /// </summary>
    public JobPriority Priority { get; set; }

    /// <summary>
    /// Whether the job is active
    /// </summary>
    public bool IsActive { get; set; }
}
