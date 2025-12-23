using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ScrapingJob entity
/// </summary>
public class ScrapingJobConfiguration : IEntityTypeConfiguration<ScrapingJob>
{
    public void Configure(EntityTypeBuilder<ScrapingJob> builder)
    {
        builder.ToTable("ScrapingJobs");

        builder.HasKey(sj => sj.Id);

        builder.Property(sj => sj.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sj => sj.TargetUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(sj => sj.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(sj => sj.Marketplace)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sj => sj.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Scheduling properties
        builder.Property(sj => sj.Frequency)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sj => sj.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sj => sj.CronExpression)
            .HasMaxLength(100);

        builder.Property(sj => sj.RunCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sj => sj.SuccessCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sj => sj.FailureCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sj => sj.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(sj => sj.MaxRetries)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(sj => sj.StartedAt)
            .IsRequired();

        builder.Property(sj => sj.ProductsScraped)
            .HasDefaultValue(0);

        builder.Property(sj => sj.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(sj => sj.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(sj => sj.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(sj => sj.StartedAt);
        builder.HasIndex(sj => sj.Status);
        builder.HasIndex(sj => sj.Marketplace);
        builder.HasIndex(sj => new { sj.IsActive, sj.NextRunAt })
            .HasDatabaseName("IX_ScrapingJobs_IsActive_NextRunAt");
    }
}
