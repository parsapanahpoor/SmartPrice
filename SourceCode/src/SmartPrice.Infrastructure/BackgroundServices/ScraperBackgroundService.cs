using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that continuously monitors and executes scheduled scraping jobs
/// </summary>
public class ScraperBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScraperBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public ScraperBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ScraperBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scraper Background Service started. Check interval: {Interval}",
            _checkInterval);

        // Wait a bit before starting to let the application fully initialize
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessJobsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service execution");

                // Wait longer before retry on error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            // Wait for the next check interval
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Scraper Background Service stopped");
    }

    private async Task ProcessJobsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();

        var jobRepository = scope.ServiceProvider.GetRequiredService<IRepository<ScrapingJob>>();
        var jobScheduler = scope.ServiceProvider.GetRequiredService<IJobScheduler>();
        var jobExecutor = scope.ServiceProvider.GetRequiredService<IJobExecutor>();

        // Get all active jobs
        var jobs = await jobRepository.FindAsync(
            j => j.IsActive &&
                 (j.Status == JobStatus.Pending || j.Status == JobStatus.Completed),
            ct);

        _logger.LogDebug("Checking {Count} active jobs for due execution", jobs.Count);

        foreach (var job in jobs)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                // Check if the job should run now
                if (await jobScheduler.ShouldRunNowAsync(job.Id, ct))
                {
                    _logger.LogInformation("Executing scheduled job: {JobName} ({JobId})",
                        job.Name, job.Id);

                    // Execute job in a separate task to avoid blocking
                    _ = Task.Run(async () =>
                    {
                        using var executionScope = _serviceProvider.CreateScope();
                        var executor = executionScope.ServiceProvider.GetRequiredService<IJobExecutor>();
                        var scheduler = executionScope.ServiceProvider.GetRequiredService<IJobScheduler>();

                        try
                        {
                            await executor.ExecuteJobAsync(job.Id, ct);
                            await scheduler.UpdateJobScheduleAsync(job.Id, ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error executing job {JobId} in background task", job.Id);
                        }
                    }, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobName} ({JobId})",
                    job.Name, job.Id);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Scraper Background Service is stopping...");
        await base.StopAsync(cancellationToken);
    }
}
