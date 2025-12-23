namespace SmartPrice.Domain.Entities;

/// <summary>
/// نشان‌دهندهٔ یک سیاق تفتیش (Audit Log) برای تتبع فعالیت‌های ادمین
/// </summary>
public class AuditLog
{
    /// <summary>
    /// شناسهٔ منحصر به فرد
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// شناسهٔ کاربر ادمینی
    /// </summary>
    public Guid AdminUserId { get; set; }

    /// <summary>
    /// عملیاتی که انجام شد
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// نوع موجودیت (Entity) تحت تأثیر
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// شناسهٔ موجودیت
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// جزئیات تغییرات
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// آدرس IP کاربر
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// زمان عملیات
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// کاربر ادمینی که این عملیات را انجام داد
    /// </summary>
    public AdminUser AdminUser { get; set; } = null!;
}
