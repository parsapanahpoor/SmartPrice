using System.ComponentModel.DataAnnotations;

namespace SmartPrice.Application.DTOs.Auth;

/// <summary>
/// DTO برای درخواست ورود
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// نام کاربری
    /// </summary>
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// رمز عبور
    /// </summary>
    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO برای جواب ورود
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// توکن دسترسی
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// توکن بازتوازن
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// زمان انقضای توکن
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// اطلاعات کاربر
    /// </summary>
    public AdminUserDto User { get; set; } = null!;
}

/// <summary>
/// DTO برای درخواست بازتوازن توکن
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// توکن بازتوازن
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO برای اطلاعات کاربر ادمین
/// </summary>
public class AdminUserDto
{
    /// <summary>
    /// شناسهٔ کاربر
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// ایمیل
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// نام کامل
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// نقش
    /// </summary>
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// DTO برای ثبت ادمین جدید
/// </summary>
public class RegisterAdminDto
{
    /// <summary>
    /// نام کاربری
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// رمز عبور
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// ایمیل
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// نام کامل
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// نقش
    /// </summary>
    [Required]
    public SmartPrice.Domain.Enums.AdminRole Role { get; set; }
}

/// <summary>
/// DTO برای تغییر رمز عبور
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// رمز عبور قدیم
    /// </summary>
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// رمز عبور جدید
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// تایید رمز عبور جدید
    /// </summary>
    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
