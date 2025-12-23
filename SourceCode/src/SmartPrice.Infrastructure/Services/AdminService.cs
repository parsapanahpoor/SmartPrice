using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.DTOs.Admin;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Infrastructure.Data;

namespace SmartPrice.Infrastructure.Services;

/// <summary>
/// سرویس مدیریت ادمین
/// </summary>
public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(ApplicationDbContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var last30Days = now.AddDays(-30);

            var stats = new DashboardStatsDto
            {
                TotalUsers = await _context.TelegramUsers.CountAsync(ct),
                ActiveUsers = await _context.TelegramUsers
                    .CountAsync(u => u.LastInteractionAt >= last30Days, ct),

                TotalProducts = await _context.Products.CountAsync(ct),
                TrackedProducts = await _context.UserProductTrackings
                    .Select(t => t.ProductId)
                    .Distinct()
                    .CountAsync(ct),

                TotalScrapingJobs = await _context.ScrapingJobs.CountAsync(ct),
                SuccessfulJobs = await _context.ScrapingJobs
                    .CountAsync(j => j.Status == Domain.Enums.JobStatus.Completed, ct),
                FailedJobs = await _context.ScrapingJobs
                    .CountAsync(j => j.Status == Domain.Enums.JobStatus.Failed, ct),

                NotificationsSent = await _context.NotificationLogs.CountAsync(ct),

                AverageResponseTime = await _context.ScrapingJobs
                    .Where(j => j.Status == Domain.Enums.JobStatus.Completed && j.Duration.HasValue)
                    .Select(j => j.Duration!.Value.TotalSeconds)
                    .DefaultIfEmpty(0)
                    .AverageAsync(ct)
            };

            // رشد کاربران (آخرین 30 روز)
            stats.UserGrowth = await _context.TelegramUsers
                .Where(u => u.CreatedAt >= last30Days)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new ChartDataDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Count()
                })
                .OrderBy(x => x.Label)
                .ToListAsync(ct);

            // تغییرات قیمت (آخرین 30 روز)
            stats.PriceChanges = await _context.PriceHistories
                .Where(p => p.RecordedAt >= last30Days)
                .GroupBy(p => p.RecordedAt.Date)
                .Select(g => new ChartDataDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = g.Count()
                })
                .OrderBy(x => x.Label)
                .ToListAsync(ct);

            _logger.LogInformation("Dashboard stats retrieved successfully");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard stats");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<UserDetailsDto>> GetAllUsersAsync(int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            return await _context.TelegramUsers
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDetailsDto
                {
                    Id = u.Id,
                    TelegramUserId = u.ChatId,
                    Username = u.Username ?? "N/A",
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    JoinedAt = u.CreatedAt,
                    LastActiveAt = u.LastInteractionAt,
                    TrackedProductsCount = u.TrackedProducts.Count,
                    NotificationsReceived = u.Notifications.Count,
                    IsActive = u.IsActive
                })
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<UserDetailsDto?> GetUserDetailsAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            return await _context.TelegramUsers
                .Where(u => u.Id == userId)
                .Select(u => new UserDetailsDto
                {
                    Id = u.Id,
                    TelegramUserId = u.ChatId,
                    Username = u.Username ?? "N/A",
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    JoinedAt = u.CreatedAt,
                    LastActiveAt = u.LastInteractionAt,
                    TrackedProductsCount = u.TrackedProducts.Count,
                    NotificationsReceived = u.Notifications.Count,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details for {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<ProductAnalyticsDto>> GetTopTrackedProductsAsync(int count, CancellationToken ct = default)
    {
        try
        {
            return await _context.Products
                .Select(p => new ProductAnalyticsDto
                {
                    ProductId = p.Id,
                    Title = p.Name,
                    Url = p.Url,
                    CurrentPrice = p.CurrentPrice,
                    TrackingUsersCount = _context.UserProductTrackings.Count(t => t.ProductId == p.Id),
                    PriceChangesCount = p.PriceHistory.Count,
                    LowestPrice = p.PriceHistory.Min(h => (decimal?)h.Price),
                    HighestPrice = p.PriceHistory.Max(h => (decimal?)h.Price),
                    LastScraped = p.LastUpdated
                })
                .OrderByDescending(p => p.TrackingUsersCount)
                .Take(count)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top tracked products");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<AuditLog>> GetAuditLogsAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        try
        {
            return await _context.AuditLogs
                .Where(a => a.CreatedAt >= from && a.CreatedAt <= to)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.TelegramUsers.FindAsync(new object[] { userId }, cancellationToken: ct);
            if (user == null)
                return false;

            user.IsActive = false;
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} deactivated", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ReactivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.TelegramUsers.FindAsync(new object[] { userId }, cancellationToken: ct);
            if (user == null)
                return false;

            user.IsActive = true;
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} reactivated", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating user {UserId}", userId);
            throw;
        }
    }
}
