using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPrice.Application.DTOs.Auth;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Enums;
using System.Security.Claims;

namespace SmartPrice.API.Controllers;

/// <summary>
/// کنترلر احراز هویت و مدیریت دسترسی
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// ورود کاربر
    /// </summary>
    /// <param name="request">درخواست ورود</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>توکن و اطلاعات کاربر</returns>
    /// <response code="200">ورود موفق</response>
    /// <response code="401">نام کاربری یا رمز عبور نادرست</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _authService.LoginAsync(request, ct);

            if (result == null)
            {
                return Unauthorized(new { message = "نام کاربری یا رمز عبور نادرست" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در سرور");
        }
    }

    /// <summary>
    /// تازه‌سازی توکن
    /// </summary>
    /// <param name="request">درخواست توکن بازتوازن</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>توکن جدید</returns>
    /// <response code="200">موفق</response>
    /// <response code="401">توکن نامعتبر</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken, ct);

            if (result == null)
            {
                return Unauthorized(new { message = "توکن بازتوازن نامعتبر یا منقضی شده" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در سرور");
        }
    }

    /// <summary>
    /// خروج کاربر
    /// </summary>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    /// <response code="200">خروج موفق</response>
    /// <response code="401">احراز هویت الزامی</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _authService.LogoutAsync(userId, ct);
            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "با موفقیت خارج شدید" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در سرور");
        }
    }

    /// <summary>
    /// ثبت ادمین جدید
    /// </summary>
    /// <param name="request">درخواست ثبت</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    /// <response code="200">ثبت موفق</response>
    /// <response code="400">نام کاربری تکراری</response>
    /// <response code="403">دسترسی رد شد</response>
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterAdminDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _authService.RegisterAdminAsync(
                request.Username,
                request.Password,
                request.Email,
                request.FullName,
                request.Role,
                ct);

            if (!result)
            {
                return BadRequest(new { message = "نام کاربری قبلاً ثبت شده است" });
            }

            return Ok(new { message = "ادمین با موفقیت ثبت شد" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering admin");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در سرور");
        }
    }

    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    /// <param name="request">درخواست تغییر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه</returns>
    /// <response code="200">موفق</response>
    /// <response code="400">رمز عبور قدیم نادرست</response>
    /// <response code="401">احراز هویت الزامی</response>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto request,
        CancellationToken ct)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword, ct);
            if (!result)
            {
                return BadRequest(new { message = "رمز عبور قدیم نادرست است" });
            }

            return Ok(new { message = "رمز عبور با موفقیت تغییر یافت" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در سرور");
        }
    }
}
