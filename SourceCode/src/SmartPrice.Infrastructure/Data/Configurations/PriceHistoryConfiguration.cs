using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for PriceHistory entity
/// </summary>
public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasKey(ph => ph.Id);

        builder.Property(ph => ph.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ph => ph.RecordedAt)
            .IsRequired();

        builder.HasIndex(ph => new { ph.ProductId, ph.RecordedAt });
    }
}
