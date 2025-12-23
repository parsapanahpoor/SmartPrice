namespace SmartPrice.Application.DTOs.Admin;

/// <summary>
/// DTO برای جزئیات کاربر
/// </summary>
public class UserDetailsDto
{
    /// <summary>
    /// شناسهٔ کاربر
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// شناسهٔ تلگرام
    /// </summary>
    public long TelegramUserId { get; set; }

    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// نام
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// نام خانوادگی
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// زمان عضویت
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// آخرین فعالیت
    /// </summary>
    public DateTime? LastActiveAt { get; set; }

    /// <summary>
    /// تعداد محصولات ترک شده
    /// </summary>
    public int TrackedProductsCount { get; set; }

    /// <summary>
    /// تعداد اطلاع‌رسانی‌های دریافت شده
    /// </summary>
    public int NotificationsReceived { get; set; }

    /// <summary>
    /// وضعیت فعالیت
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO برای تجزیه و تحلیل محصول
/// </summary>
public class ProductAnalyticsDto
{
    /// <summary>
    /// شناسهٔ محصول
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// عنوان محصول
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL محصول
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// قیمت فعلی
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// تعداد کاربران تتبع
    /// </summary>
    public int TrackingUsersCount { get; set; }

    /// <summary>
    /// تعداد تغییرات قیمت
    /// </summary>
    public int PriceChangesCount { get; set; }

    /// <summary>
    /// کمترین قیمت
    /// </summary>
    public decimal? LowestPrice { get; set; }

    /// <summary>
    /// بیشترین قیمت
    /// </summary>
    public decimal? HighestPrice { get; set; }

    /// <summary>
    /// آخرین اسکریپینگ
    /// </summary>
    public DateTime? LastScraped { get; set; }
}
