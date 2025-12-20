using Microsoft.AspNetCore.Mvc;
using SmartPrice.Application.DTOs.Jobs;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.API.Controllers;

/// <summary>
/// Job management endpoints for scheduling and monitoring scraping jobs
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class JobsController : ControllerBase
{
    private readonly IRepository<ScrapingJob> _jobRepository;
    private readonly IScrapingQueueService _queueService;
    private readonly IJobScheduler _jobScheduler;
    private readonly IJobExecutor _jobExecutor;
    private readonly ILogger<JobsController> _logger;

    public JobsController(
        IRepository<ScrapingJob> jobRepository,
        IScrapingQueueService queueService,
        IJobScheduler jobScheduler,
        IJobExecutor jobExecutor,
        ILogger<JobsController> logger)
    {
        _jobRepository = jobRepository;
        _queueService = queueService;
        _jobScheduler = jobScheduler;
        _jobExecutor = jobExecutor;
        _logger = logger;
    }

    /// <summary>
    /// Create a new scraping job
    /// </summary>
    /// <param name="dto">Job creation details</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created job details</returns>
    /// <response code="201">Job created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { error = "Job name is required" });
        }

        if (dto.Urls == null || !dto.Urls.Any())
        {
            return BadRequest(new { error = "At least one URL is required" });
        }

        // Validate cron expression if custom frequency
        if (dto.Frequency == JobFrequency.Custom && string.IsNullOrWhiteSpace(dto.CronExpression))
        {
            return BadRequest(new { error = "Cron expression is required for custom frequency" });
        }

        var job = new ScrapingJob
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            TargetUrl = dto.Urls.First(), // Primary URL
            Frequency = dto.Frequency,
            CronExpression = dto.CronExpression,
            Priority = dto.Priority,
            IsActive = dto.IsActive,
            MaxRetries = dto.MaxRetries,
            Status = JobStatus.Pending,
            Marketplace = MarketplaceType.Other, // Will be detected
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            StartedAt = DateTime.UtcNow
        };

        await _jobRepository.AddAsync(job, ct);

        _logger.LogInformation("Created job: {JobName} ({JobId}) with {Count} URLs",
            dto.Name, job.Id, dto.Urls.Count);

        // Enqueue all URLs
        foreach (var url in dto.Urls)
        {
            await _queueService.EnqueueAsync(url, job.Id, dto.Priority, ct);
        }

        // Schedule the job
        await _jobScheduler.ScheduleJobAsync(job.Id, ct);

        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, new
        {
            jobId = job.Id,
            name = job.Name,
            frequency = job.Frequency.ToString(),
            priority = job.Priority.ToString(),
            isActive = job.IsActive,
            nextRunAt = job.NextRunAt,
            urlCount = dto.Urls.Count
        });
    }

    /// <summary>
    /// Get job status and details
    /// </summary>
    /// <param name="id">Job identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Job status information</returns>
    /// <response code="200">Job found</response>
    /// <response code="404">Job not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJob(Guid id, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job == null)
        {
            return NotFound(new { error = "Job not found" });
        }

        var queueLength = await _queueService.GetQueueLengthAsync(ct);

        var status = new JobStatusDto
        {
            JobId = job.Id,
            Name = job.Name,
            Status = job.Status,
            NextRunAt = job.NextRunAt,
            LastRunAt = job.LastRunAt,
            QueueLength = queueLength,
            TotalRuns = job.RunCount,
            SuccessCount = job.SuccessCount,
            FailureCount = job.FailureCount,
            Frequency = job.Frequency,
            Priority = job.Priority,
            IsActive = job.IsActive
        };

        return Ok(status);
    }

    /// <summary>
    /// Get all jobs
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of all jobs</returns>
    /// <response code="200">Jobs retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllJobs(CancellationToken ct)
    {
        var jobs = await _jobRepository.ListAsync(ct);

        var jobDtos = jobs.Select(j => new
        {
            jobId = j.Id,
            name = j.Name,
            status = j.Status.ToString(),
            frequency = j.Frequency.ToString(),
            priority = j.Priority.ToString(),
            isActive = j.IsActive,
            nextRunAt = j.NextRunAt,
            lastRunAt = j.LastRunAt,
            totalRuns = j.RunCount,
            successCount = j.SuccessCount,
            failureCount = j.FailureCount
        }).ToList();

        return Ok(new
        {
            total = jobDtos.Count,
            jobs = jobDtos
        });
    }

    /// <summary>
    /// Execute a job immediately (manual trigger)
    /// </summary>
    /// <param name="id">Job identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Execution started confirmation</returns>
    /// <response code="202">Execution started</response>
    /// <response code="404">Job not found</response>
    /// <response code="409">Job is already running</response>
    [HttpPost("{id}/execute")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ExecuteJob(Guid id, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job == null)
        {
            return NotFound(new { error = "Job not found" });
        }

        if (job.Status == JobStatus.Running)
        {
            return Conflict(new { error = "Job is already running" });
        }

        _logger.LogInformation("Manual execution triggered for job: {JobName} ({JobId})",
            job.Name, id);

        // Execute job in background
        _ = Task.Run(async () =>
        {
            try
            {
                await _jobExecutor.ExecuteJobAsync(id, ct);
                await _jobScheduler.UpdateJobScheduleAsync(id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing job {JobId}", id);
            }
        }, ct);

        return Accepted(new
        {
            message = "Job execution started",
            jobId = id,
            jobName = job.Name
        });
    }

    /// <summary>
    /// Update job active status
    /// </summary>
    /// <param name="id">Job identifier</param>
    /// <param name="request">Activation request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated job status</returns>
    /// <response code="200">Status updated</response>
    /// <response code="404">Job not found</response>
    [HttpPatch("{id}/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJobStatus(
        Guid id,
        [FromBody] UpdateJobStatusRequest request,
        CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job == null)
        {
            return NotFound(new { error = "Job not found" });
        }

        job.IsActive = request.IsActive;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job, ct);

        _logger.LogInformation("Job {JobName} ({JobId}) active status changed to: {IsActive}",
            job.Name, id, request.IsActive);

        return Ok(new
        {
            jobId = id,
            jobName = job.Name,
            isActive = job.IsActive,
            message = job.IsActive ? "Job activated" : "Job deactivated"
        });
    }

    /// <summary>
    /// Delete a job
    /// </summary>
    /// <param name="id">Job identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="204">Job deleted</response>
    /// <response code="404">Job not found</response>
    /// <response code="409">Cannot delete running job</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteJob(Guid id, CancellationToken ct)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job == null)
        {
            return NotFound(new { error = "Job not found" });
        }

        if (job.Status == JobStatus.Running)
        {
            return Conflict(new { error = "Cannot delete a running job" });
        }

        await _jobRepository.DeleteAsync(job, ct);

        _logger.LogInformation("Job deleted: {JobName} ({JobId})", job.Name, id);

        return NoContent();
    }
}

/// <summary>
/// Request model for updating job status
/// </summary>
public class UpdateJobStatusRequest
{
    /// <summary>
    /// Whether the job should be active
    /// </summary>
    public bool IsActive { get; set; }
}
