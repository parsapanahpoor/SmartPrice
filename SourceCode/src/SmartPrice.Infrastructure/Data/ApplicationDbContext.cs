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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
