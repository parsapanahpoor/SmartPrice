using SmartPrice.Application.DTOs.Auth;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// سرویس احراز هویت
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// ورود کاربر
    /// </summary>
    /// <param name="request">درخواست ورود</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>جواب ورود</returns>
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken ct = default);

    /// <summary>
    /// تازه‌سازی توکن
    /// </summary>
    /// <param name="refreshToken">توکن بازتوازن</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>جواب ورود جدید</returns>
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// خروج کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    Task<bool> LogoutAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// ثبت ادمین جدید
    /// </summary>
    /// <param name="username">نام کاربری</param>
    /// <param name="password">رمز عبور</param>
    /// <param name="email">ایمیل</param>
    /// <param name="fullName">نام کامل</param>
    /// <param name="role">نقش</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    Task<bool> RegisterAdminAsync(string username, string password, string email, string fullName, AdminRole role, CancellationToken ct = default);

    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="oldPassword">رمز عبور قدیم</param>
    /// <param name="newPassword">رمز عبور جدید</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword, CancellationToken ct = default);
}
