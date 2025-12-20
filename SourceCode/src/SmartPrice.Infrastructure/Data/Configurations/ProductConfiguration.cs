using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Product entity
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(p => p.Url)
            .IsUnique();

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(2000);

        builder.Property(p => p.Category)
            .HasMaxLength(200);

        builder.Property(p => p.CurrentPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.OriginalPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.DiscountPercentage)
            .HasDefaultValue(0);

        builder.Property(p => p.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(p => p.LastUpdated)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasMany(p => p.PriceHistory)
            .WithOne(ph => ph.Product)
            .HasForeignKey(ph => ph.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
