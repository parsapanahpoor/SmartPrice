using SmartPrice.Application.DTOs.Admin;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// سرویس تجزیه و تحلیل سیستم
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// ثبت متریک
    /// </summary>
    /// <param name="type">نوع متریک</param>
    /// <param name="metricName">نام متریک</param>
    /// <param name="value">مقدار</param>
    /// <param name="details">جزئیات</param>
    /// <param name="ct">توکن لغو</param>
    Task TrackMetricAsync(MetricType type, string metricName, double value, string? details = null, CancellationToken ct = default);

    /// <summary>
    /// دریافت متریک‌ها
    /// </summary>
    /// <param name="type">نوع متریک</param>
    /// <param name="from">از تاریخ</param>
    /// <param name="to">تا تاریخ</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>لیست متریک‌ها</returns>
    Task<List<Domain.Entities.SystemMetric>> GetMetricsAsync(MetricType type, DateTime from, DateTime to, CancellationToken ct = default);

    /// <summary>
    /// دریافت میانگین متریک‌ها
    /// </summary>
    /// <param name="type">نوع متریک</param>
    /// <param name="days">تعداد روزها</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>فرهنگ میانگین‌ها</returns>
    Task<Dictionary<string, double>> GetAverageMetricsAsync(MetricType type, int days, CancellationToken ct = default);

    /// <summary>
    /// دریافت وضعیت سلامت سیستم
    /// </summary>
    /// <param name="ct">توکن لغو</param>
    /// <returns>وضعیت سلامت</returns>
    Task<SystemHealthDto> GetSystemHealthAsync(CancellationToken ct = default);
}
