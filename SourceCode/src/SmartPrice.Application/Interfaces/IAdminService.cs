using SmartPrice.Application.DTOs.Admin;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// سرویس مدیریت ادمین
/// </summary>
public interface IAdminService
{
    /// <summary>
    /// دریافت آمار داشبورد
    /// </summary>
    /// <param name="ct">توکن لغو</param>
    /// <returns>آمار داشبورد</returns>
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default);

    /// <summary>
    /// دریافت تمام کاربران
    /// </summary>
    /// <param name="page">صفحه</param>
    /// <param name="pageSize">اندازهٔ صفحه</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>لیست کاربران</returns>
    Task<List<UserDetailsDto>> GetAllUsersAsync(int page, int pageSize, CancellationToken ct = default);

    /// <summary>
    /// دریافت جزئیات کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>جزئیات کاربر</returns>
    Task<UserDetailsDto?> GetUserDetailsAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// دریافت محبوب‌ترین محصولات
    /// </summary>
    /// <param name="count">تعداد</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>لیست محصولات</returns>
    Task<List<ProductAnalyticsDto>> GetTopTrackedProductsAsync(int count, CancellationToken ct = default);

    /// <summary>
    /// دریافت سیاق‌های تفتیش
    /// </summary>
    /// <param name="from">از تاریخ</param>
    /// <param name="to">تا تاریخ</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>لیست سیاق‌ها</returns>
    Task<List<AuditLog>> GetAuditLogsAsync(DateTime from, DateTime to, CancellationToken ct = default);

    /// <summary>
    /// غیرفعال کردن کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// فعال کردن مجدد کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    Task<bool> ReactivateUserAsync(Guid userId, CancellationToken ct = default);
}
