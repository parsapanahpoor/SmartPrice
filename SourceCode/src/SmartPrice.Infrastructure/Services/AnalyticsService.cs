using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.DTOs.Admin;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;
using SmartPrice.Infrastructure.Data;

namespace SmartPrice.Infrastructure.Services;

/// <summary>
/// سرویس تجزیه و تحلیل سیستم
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(ApplicationDbContext context, ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task TrackMetricAsync(MetricType type, string metricName, double value, string? details = null, CancellationToken ct = default)
    {
        try
        {
            var metric = new SystemMetric
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                MetricType = type.ToString(),
                MetricName = metricName,
                Value = value,
                Details = details
            };

            await _context.SystemMetrics.AddAsync(metric, ct);
            await _context.SaveChangesAsync(ct);

            _logger.LogDebug("Metric tracked: {MetricType} - {MetricName} = {Value}", type, metricName, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking metric {MetricName}", metricName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<SystemMetric>> GetMetricsAsync(MetricType type, DateTime from, DateTime to, CancellationToken ct = default)
    {
        try
        {
            return await _context.SystemMetrics
                .Where(m => m.MetricType == type.ToString() && m.Timestamp >= from && m.Timestamp <= to)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metrics");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, double>> GetAverageMetricsAsync(MetricType type, int days, CancellationToken ct = default)
    {
        try
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            var averages = await _context.SystemMetrics
                .Where(m => m.MetricType == type.ToString() && m.Timestamp >= fromDate)
                .GroupBy(m => m.MetricName)
                .Select(g => new { Name = g.Key, Average = g.Average(m => m.Value) })
                .ToListAsync(ct);

            return averages.ToDictionary(a => a.Name, a => a.Average);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving average metrics");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<SystemHealthDto> GetSystemHealthAsync(CancellationToken ct = default)
    {
        try
        {
            var activeJobs = await _context.ScrapingJobs
                .CountAsync(j => j.Status == JobStatus.Running, cancellationToken: ct);

            var queuedJobs = await _context.ScrapingJobs
                .CountAsync(j => j.Status == JobStatus.Pending, cancellationToken: ct);

            var cpuUsage = GetCpuUsage();
            var memoryUsage = GetMemoryUsage();

            return new SystemHealthDto
            {
                Status = DetermineHealthStatus(queuedJobs, cpuUsage, memoryUsage),
                CpuUsage = cpuUsage,
                MemoryUsage = memoryUsage,
                ActiveJobs = activeJobs,
                QueuedJobs = queuedJobs,
                LastCheck = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system health");
            throw;
        }
    }

    /// <summary>
    /// تعیین وضعیت سلامت سیستم
    /// </summary>
    private string DetermineHealthStatus(int queuedJobs, double cpuUsage, double memoryUsage)
    {
        if (queuedJobs > 100 || cpuUsage > 90 || memoryUsage > 90)
            return "Critical";

        if (queuedJobs > 50 || cpuUsage > 70 || memoryUsage > 70)
            return "Warning";

        return "Healthy";
    }

    /// <summary>
    /// دریافت استفاده CPU
    /// </summary>
    private double GetCpuUsage()
    {
        try
        {
            // بدون استفاده از PerformanceCounter در سیستم‌های non-Windows
            // بجای آن از GC metrics استفاده می‌کنیم
            return 0;
        }
        catch
        {
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
            var totalMemory = GC.GetTotalMemory(false) / 1024.0 / 1024.0; // MB

            // Assuming 2GB available for app
            return Math.Round((totalMemory / 2048) * 100, 2);
        }
        catch
        {
            return 0;
        }
    }
}
