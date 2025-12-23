namespace SmartPrice.Application.DTOs.Admin;

/// <summary>
/// DTO برای نمایش آمار داشبورد
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// کل کاربران
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// کاربران فعال
    /// </summary>
    public int ActiveUsers { get; set; }

    /// <summary>
    /// کل محصولات
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// محصولات ترک شده
    /// </summary>
    public int TrackedProducts { get; set; }

    /// <summary>
    /// کل کارهای اسکریپینگ
    /// </summary>
    public int TotalScrapingJobs { get; set; }

    /// <summary>
    /// کارهای موفق
    /// </summary>
    public int SuccessfulJobs { get; set; }

    /// <summary>
    /// کارهای ناموفق
    /// </summary>
    public int FailedJobs { get; set; }

    /// <summary>
    /// اطلاع‌رسانی‌های ارسال شده
    /// </summary>
    public int NotificationsSent { get; set; }

    /// <summary>
    /// میانگین زمان پاسخ (ثانیه)
    /// </summary>
    public double AverageResponseTime { get; set; }

    /// <summary>
    /// داده‌های رشد کاربران
    /// </summary>
    public List<ChartDataDto> UserGrowth { get; set; } = new();

    /// <summary>
    /// داده‌های تغییرات قیمت
    /// </summary>
    public List<ChartDataDto> PriceChanges { get; set; } = new();
}

/// <summary>
/// DTO برای داده‌های نمودار
/// </summary>
public class ChartDataDto
{
    /// <summary>
    /// برچسب نمودار
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// مقدار
    /// </summary>
    public double Value { get; set; }
}
