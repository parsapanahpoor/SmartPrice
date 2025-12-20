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
        builder.HasKey(sj => sj.Id);

        builder.Property(sj => sj.TargetUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(sj => sj.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(sj => sj.StartedAt)
            .IsRequired();

        builder.Property(sj => sj.ProductsScraped)
            .HasDefaultValue(0);

        builder.Property(sj => sj.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(sj => sj.StartedAt);
        builder.HasIndex(sj => sj.Status);
    }
}
