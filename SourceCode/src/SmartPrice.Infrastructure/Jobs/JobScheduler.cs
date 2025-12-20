using Cronos;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Jobs;

/// <summary>
/// Service for scheduling and managing job execution timing
/// </summary>
public class JobScheduler : IJobScheduler
{
    private readonly IRepository<ScrapingJob> _jobRepository;
    private readonly ILogger<JobScheduler> _logger;

    public JobScheduler(
        IRepository<ScrapingJob> jobRepository,
        ILogger<JobScheduler> logger)
    {
        _jobRepository = jobRepository;
        _logger = logger;
    }

    public async Task<DateTime?> GetNextRunTimeAsync(Guid jobId, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, ct);
        if (job == null || !job.IsActive) return null;

        var now = DateTime.UtcNow;
        var lastRun = job.LastRunAt ?? job.CreatedAt;

        return job.Frequency switch
        {
            JobFrequency.Manual => null, // Manual jobs don't have automatic scheduling
            JobFrequency.Hourly => lastRun.AddHours(1),
            JobFrequency.Daily => lastRun.AddDays(1),
            JobFrequency.Weekly => lastRun.AddDays(7),
            JobFrequency.Custom when !string.IsNullOrEmpty(job.CronExpression)
                => CalculateNextRunFromCron(job.CronExpression, lastRun),
            _ => null
        };
    }

    public async Task<bool> ShouldRunNowAsync(Guid jobId, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, ct);
        if (job == null || !job.IsActive)
        {
            return false;
        }

        // Don't run if already in progress
        if (job.Status == JobStatus.Running)
        {
            _logger.LogDebug("Job {JobId} is already running", jobId);
            return false;
        }

        var nextRun = await GetNextRunTimeAsync(jobId, ct);

        // Manual jobs don't run automatically
        if (job.Frequency == JobFrequency.Manual)
        {
            return false;
        }

        // Check if it's time to run
        var shouldRun = nextRun.HasValue && nextRun.Value <= DateTime.UtcNow;

        if (shouldRun)
        {
            _logger.LogInformation("Job {JobName} ({JobId}) is due for execution", job.Name, jobId);
        }

        return shouldRun;
    }

    public async Task UpdateJobScheduleAsync(Guid jobId, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, ct);
        if (job == null) return;

        job.LastRunAt = DateTime.UtcNow;
        job.NextRunAt = await GetNextRunTimeAsync(jobId, ct);
        job.RunCount++;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job, ct);

        _logger.LogInformation("Job schedule updated: {JobName} ({JobId}). Next run: {NextRun}",
            job.Name, jobId, job.NextRunAt);
    }

    public async Task ScheduleJobAsync(Guid jobId, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, ct);
        if (job == null) return;

        job.NextRunAt = await GetNextRunTimeAsync(jobId, ct);
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job, ct);

        _logger.LogInformation("Job scheduled: {JobName} ({JobId}) for {NextRun}",
            job.Name, jobId, job.NextRunAt);
    }

    private DateTime? CalculateNextRunFromCron(string cronExpression, DateTime fromDate)
    {
        try
        {
            var cron = CronExpression.Parse(cronExpression);
            var nextOccurrence = cron.GetNextOccurrence(fromDate, TimeZoneInfo.Utc);

            if (nextOccurrence.HasValue)
            {
                _logger.LogDebug("Next run calculated from cron '{Cron}': {NextRun}",
                    cronExpression, nextOccurrence.Value);
            }

            return nextOccurrence;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invalid cron expression: {Cron}", cronExpression);
            return null;
        }
    }
}
