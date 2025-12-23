namespace SmartPrice.Domain.Entities;

using SmartPrice.Domain.Enums;

/// <summary>
/// نشان‌دهندهٔ یک کاربر ادمین در سیستم
/// </summary>
public class AdminUser
{
    /// <summary>
    /// شناسهٔ منحصر به فرد
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// هش رمز عبور
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// آدرس ایمیل
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// نام کامل
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// نقش کاربری
    /// </summary>
    public AdminRole Role { get; set; }

    /// <summary>
    /// وضعیت فعالیت
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// آخرین زمان ورود
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// توکن بازتوازن
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// زمان انقضای توکن بازتوازن
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// زمان ایجاد
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// زمان آخرین بروزرسانی
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    /// <summary>
    /// سیاق‌های تفتیش انجام شده توسط این ادمین
    /// </summary>
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
