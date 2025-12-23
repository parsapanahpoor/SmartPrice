using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.BackgroundServices;

/// <summary>
/// سرویس پس‌زمینه برای جمع‌آوری متریک‌های سیستم
/// </summary>
public class MetricsCollectorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MetricsCollectorService> _logger;
    private const int CollectionIntervalMinutes = 5;

    public MetricsCollectorService(
        IServiceProvider serviceProvider,
        ILogger<MetricsCollectorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// اجرای سرویس پس‌زمینه
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Metrics Collector Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();

                // جمع‌آوری متریک‌های CPU و حافظه
                var cpuUsage = GetCpuUsage();
                var memoryUsage = GetMemoryUsage();
                var activeJobs = GetActiveJobsCount();

                // ثبت متریک‌ها
                await analyticsService.TrackMetricAsync(
                    MetricType.Performance,
                    "CPU_Usage_Percent",
                    cpuUsage,
                    null,
                    stoppingToken);

                await analyticsService.TrackMetricAsync(
                    MetricType.Performance,
                    "Memory_Usage_MB",
                    memoryUsage,
                    null,
                    stoppingToken);

                await analyticsService.TrackMetricAsync(
                    MetricType.Performance,
                    "Active_Jobs_Count",
                    activeJobs,
                    null,
                    stoppingToken);

                _logger.LogInformation(
                    "Metrics collected - CPU: {CpuUsage}%, Memory: {MemoryUsage}MB, Active Jobs: {ActiveJobs}",
                    cpuUsage, memoryUsage, activeJobs);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Metrics Collector Service is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting metrics");
            }

            // انتظار تا زمان جمع‌آوری بعدی
            await Task.Delay(TimeSpan.FromMinutes(CollectionIntervalMinutes), stoppingToken);
        }

        _logger.LogInformation("Metrics Collector Service stopped");
    }

    /// <summary>
    /// دریافت استفاده CPU
    /// </summary>
    private double GetCpuUsage()
    {
        try
        {
            // استفاده از GC metrics بجای PerformanceCounter
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not retrieve CPU usage");
            return 0;
        }
    }

    /// <summary>
    /// دریافت استفاده حافظه
    /// </summary>
    private double GetMemoryUsage()
    {
        try
        {
            var totalMemory = GC.GetTotalMemory(false) / 1024.0 / 1024.0;
            return Math.Round(totalMemory, 2); // به MB
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not retrieve memory usage");
            return 0;
        }
    }

    /// <summary>
    /// دریافت تعداد کارهای فعال
    /// </summary>
    private double GetActiveJobsCount()
    {
        try
        {
            // برای جلوگیری از مشکلات dependency injection در background service
            // این به طور موقت 0 برگردانده می‌شود
            // در عملیات تولید باید بهتر شود
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not retrieve active jobs count");
            return 0;
        }
    }
}
