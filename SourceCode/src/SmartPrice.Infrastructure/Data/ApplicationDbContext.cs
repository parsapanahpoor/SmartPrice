using Microsoft.EntityFrameworkCore;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Products table
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Price history table
    /// </summary>
    public DbSet<PriceHistory> PriceHistories { get; set; } = null!;

    /// <summary>
    /// Telegram channels table
    /// </summary>
    public DbSet<TelegramChannel> TelegramChannels { get; set; } = null!;

    /// <summary>
    /// Scraping jobs table
    /// </summary>
    public DbSet<ScrapingJob> ScrapingJobs { get; set; } = null!;

    /// <summary>
    /// Proxy servers table
    /// </summary>
    public DbSet<ProxyServer> ProxyServers { get; set; } = null!;

    /// <summary>
    /// Scraping queue table
    /// </summary>
    public DbSet<ScrapingQueue> ScrapingQueues { get; set; } = null!;

    /// <summary>
    /// Telegram users table
    /// </summary>
    public DbSet<TelegramUser> TelegramUsers { get; set; } = null!;

    /// <summary>
    /// User product tracking table
    /// </summary>
    public DbSet<UserProductTracking> UserProductTrackings { get; set; } = null!;

    /// <summary>
    /// Notification logs table
    /// </summary>
    public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;

    /// <summary>
    /// Admin users table
    /// </summary>
    public DbSet<AdminUser> AdminUsers { get; set; } = null!;

    /// <summary>
    /// Audit logs table
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    /// <summary>
    /// System metrics table
    /// </summary>
    public DbSet<SystemMetric> SystemMetrics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
