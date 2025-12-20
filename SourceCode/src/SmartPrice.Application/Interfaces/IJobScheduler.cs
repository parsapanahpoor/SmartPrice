namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Service for scheduling and managing job execution timing
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Schedule a job for future execution
    /// </summary>
    /// <param name="jobId">The job identifier</param>
    /// <param name="ct">Cancellation token</param>
    Task ScheduleJobAsync(Guid jobId, CancellationToken ct);

    /// <summary>
    /// Calculate the next run time for a job
    /// </summary>
    /// <param name="jobId">The job identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Next scheduled run time or null if not scheduled</returns>
    Task<DateTime?> GetNextRunTimeAsync(Guid jobId, CancellationToken ct);

    /// <summary>
    /// Determine if a job should run now
    /// </summary>
    /// <param name="jobId">The job identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if the job should run now</returns>
    Task<bool> ShouldRunNowAsync(Guid jobId, CancellationToken ct);

    /// <summary>
    /// Update job schedule after execution
    /// </summary>
    /// <param name="jobId">The job identifier</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateJobScheduleAsync(Guid jobId, CancellationToken ct);
}
