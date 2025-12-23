using Microsoft.AspNetCore.Mvc;
using SmartPrice.Application.DTOs.Admin;
using SmartPrice.Application.Interfaces;

namespace SmartPrice.API.Controllers;

/// <summary>
/// کنترلر مدیریت ادمین
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IAdminService adminService,
        IAnalyticsService analyticsService,
        ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// دریافت آمار داشبورد
    /// </summary>
    /// <param name="ct">توکن لغو</param>
    /// <returns>آمار داشبورد</returns>
    /// <response code="200">آمار با موفقیت دریافت شد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboard(CancellationToken ct)
    {
        try
        {
            var stats = await _adminService.GetDashboardStatsAsync(ct);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard stats");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در دریافت آمار داشبورد");
        }
    }

    /// <summary>
    /// دریافت لیست کاربران
    /// </summary>
    /// <param name="page">شماره صفحه (پیش‌فرض: 1)</param>
    /// <param name="pageSize">تعداد نتایج در صفحه (پیش‌فرض: 20)</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>لیست کاربران</returns>
    /// <response code="200">لیست با موفقیت دریافت شد</response>
    /// <response code="400">درخواست نامعتبر</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<UserDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UserDetailsDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("صفحه و اندازهٔ صفحه باید بزرگتر از صفر باشند");

            var users = await _adminService.GetAllUsersAsync(page, pageSize, ct);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در دریافت کاربران");
        }
    }

    /// <summary>
    /// دریافت جزئیات کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>جزئیات کاربر</returns>
    /// <response code="200">جزئیات با موفقیت دریافت شد</response>
    /// <response code="404">کاربر یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDetailsDto>> GetUserDetails(
        Guid userId,
        CancellationToken ct)
    {
        try
        {
            var user = await _adminService.GetUserDetailsAsync(userId, ct);
            if (user == null)
                return NotFound("کاربر یافت نشد");

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details for {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در دریافت جزئیات کاربر");
        }
    }

    /// <summary>
    /// دریافت محبوب‌ترین محصولات
    /// </summary>
    /// <param name="count">تعداد محصولات (پیش‌فرض: 10)</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>لیست محصولات</returns>
    /// <response code="200">لیست با موفقیت دریافت شد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("products/top")]
    [ProducesResponseType(typeof(List<ProductAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductAnalyticsDto>>> GetTopProducts(
        [FromQuery] int count = 10,
        CancellationToken ct = default)
    {
        try
        {
            if (count <= 0)
                count = 10;

            var products = await _adminService.GetTopTrackedProductsAsync(count, ct);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top products");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در دریافت محصولات");
        }
    }

    /// <summary>
    /// دریافت وضعیت سلامت سیستم
    /// </summary>
    /// <param name="ct">توکن لغو</param>
    /// <returns>وضعیت سلامت</returns>
    /// <response code="200">وضعیت با موفقیت دریافت شد</response>
    /// <response code="500">خطای سرور</response>
    [HttpGet("health")]
    [ProducesResponseType(typeof(SystemHealthDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SystemHealthDto>> GetHealth(CancellationToken ct)
    {
        try
        {
            var health = await _analyticsService.GetSystemHealthAsync(ct);
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system health");
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در دریافت وضعیت سلامت سیستم");
        }
    }

    /// <summary>
    /// غیرفعال کردن کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه عملیات</returns>
    /// <response code="200">کاربر با موفقیت غیرفعال شد</response>
    /// <response code="404">کاربر یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("users/{userId:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateUser(
        Guid userId,
        CancellationToken ct)
    {
        try
        {
            var result = await _adminService.DeactivateUserAsync(userId, ct);
            if (!result)
                return NotFound("کاربر یافت نشد");

            return Ok("کاربر با موفقیت غیرفعال شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در غیرفعال کردن کاربر");
        }
    }

    /// <summary>
    /// فعال کردن مجدد کاربر
    /// </summary>
    /// <param name="userId">شناسهٔ کاربر</param>
    /// <param name="ct">توکن لغو</param>
    /// <returns>نتیجه عملیات</returns>
    /// <response code="200">کاربر با موفقیت فعال شد</response>
    /// <response code="404">کاربر یافت نشد</response>
    /// <response code="500">خطای سرور</response>
    [HttpPost("users/{userId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReactivateUser(
        Guid userId,
        CancellationToken ct)
    {
        try
        {
            var result = await _adminService.ReactivateUserAsync(userId, ct);
            if (!result)
                return NotFound("کاربر یافت نشد");

            return Ok("کاربر با موفقیت فعال شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "خطا در فعال کردن کاربر");
        }
    }
}
