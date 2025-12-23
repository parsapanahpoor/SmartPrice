namespace SmartPrice.Domain.Enums;

/// <summary>
/// نقش‌های مختلف ادمین
/// </summary>
public enum AdminRole
{
    /// <summary>
    /// سوپر ادمین - دسترسی کامل
    /// </summary>
    SuperAdmin = 1,

    /// <summary>
    /// ادمین - دسترسی به اکثر عملیات
    /// </summary>
    Admin = 2,

    /// <summary>
    /// تدوین‌کننده - دسترسی محدود
    /// </summary>
    Moderator = 3,

    /// <summary>
    /// بیننده - فقط مشاهده
    /// </summary>
    Viewer = 4
}

/// <summary>
/// انواع متریک‌های سیستم
/// </summary>
public enum MetricType
{
    /// <summary>
    /// متریک‌های اسکریپینگ
    /// </summary>
    Scraping = 1,

    /// <summary>
    /// متریک‌های اطلاع‌رسانی
    /// </summary>
    Notifications = 2,

    /// <summary>
    /// متریک‌های کاربران
    /// </summary>
    Users = 3,

    /// <summary>
    /// متریک‌های عملکرد
    /// </summary>
    Performance = 4,

    /// <summary>
    /// متریک‌های خطاها
    /// </summary>
    Errors = 5
}
