namespace SmartPrice.Application.DTOs.Admin;

/// <summary>
/// DTO برای وضعیت سلامت سیستم
/// </summary>
public class SystemHealthDto
{
    /// <summary>
    /// وضعیت کلی
    /// </summary>
    public string Status { get; set; } = "Healthy";

    /// <summary>
    /// استفاده CPU (درصد)
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// استفاده حافظه (درصد)
    /// </summary>
    public double MemoryUsage { get; set; }

    /// <summary>
    /// کارهای فعال
    /// </summary>
    public int ActiveJobs { get; set; }

    /// <summary>
    /// کارهای در صف انتظار
    /// </summary>
    public int QueuedJobs { get; set; }

    /// <summary>
    /// آخرین بررسی
    /// </summary>
    public DateTime LastCheck { get; set; }
}

/// <summary>
/// DTO برای ورود ادمین
/// </summary>
public class AdminLoginDto
{
    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// رمز عبور
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO برای جواب ورود
/// </summary>
public class AdminLoginResponseDto
{
    /// <summary>
    /// توکن دسترسی
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// نقش
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// زمان انقضا
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
