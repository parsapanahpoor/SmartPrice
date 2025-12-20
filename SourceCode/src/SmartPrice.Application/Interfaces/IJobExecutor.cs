namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Service for executing scraping jobs
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Execute a complete scraping job
    /// </summary>
    /// <param name="jobId">The job identifier</param>
    /// <param name="ct">Cancellation token</param>
    Task ExecuteJobAsync(Guid jobId, CancellationToken ct);

    /// <summary>
    /// Execute scraping for a single URL
    /// </summary>
    /// <param name="url">URL to scrape</param>
    /// <param name="jobId">Parent job identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Execution result</returns>
    Task<JobExecutionResult> ExecuteUrlAsync(string url, Guid jobId, CancellationToken ct);
}

/// <summary>
/// Result of a job execution
/// </summary>
public class JobExecutionResult
{
    /// <summary>
    /// Whether the execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of items successfully processed
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Number of items that failed
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Total execution duration
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// List of error messages
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
