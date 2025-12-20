using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ScrapingQueue entity
/// </summary>
public class ScrapingQueueConfiguration : IEntityTypeConfiguration<ScrapingQueue>
{
    public void Configure(EntityTypeBuilder<ScrapingQueue> builder)
    {
        builder.ToTable("ScrapingQueues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Marketplace)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Result)
            .HasColumnType("text");

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Composite index for efficient queue queries
        builder.HasIndex(x => new { x.Status, x.Priority, x.ScheduledAt })
            .HasDatabaseName("IX_ScrapingQueues_Status_Priority_ScheduledAt");

        // Index for job queries
        builder.HasIndex(x => x.ScrapingJobId);

        // Foreign key relationship
        builder.HasOne(x => x.ScrapingJob)
            .WithMany()
            .HasForeignKey(x => x.ScrapingJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
