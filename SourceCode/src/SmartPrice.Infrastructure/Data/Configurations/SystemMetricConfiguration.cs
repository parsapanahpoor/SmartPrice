using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی موجودیت SystemMetric
/// </summary>
public class SystemMetricConfiguration : IEntityTypeConfiguration<SystemMetric>
{
    public void Configure(EntityTypeBuilder<SystemMetric> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.MetricType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.MetricName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.Details)
            .HasMaxLength(1000);

        // Indexes for efficient querying
        builder.HasIndex(x => x.MetricType);
        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => new { x.MetricType, x.Timestamp });

        // Table settings
        builder.ToTable(t =>
        {
            // Add a comment for documentation
            t.HasComment("Stores system performance and operational metrics");
        });
    }
}
