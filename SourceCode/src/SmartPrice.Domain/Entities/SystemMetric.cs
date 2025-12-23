namespace SmartPrice.Domain.Entities;

/// <summary>
/// نشان‌دهندهٔ یک متریک سیستم برای نظارت بر عملکرد
/// </summary>
public class SystemMetric
{
    /// <summary>
    /// شناسهٔ منحصر به فرد
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// مهر زمانی
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// نوع متریک
    /// </summary>
    public string MetricType { get; set; } = string.Empty;

    /// <summary>
    /// نام متریک
    /// </summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>
    /// مقدار متریک
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// جزئیات اضافی
    /// </summary>
    public string? Details { get; set; }
}
