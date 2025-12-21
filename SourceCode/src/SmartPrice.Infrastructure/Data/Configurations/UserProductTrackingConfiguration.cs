using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for UserProductTracking entity
/// </summary>
public class UserProductTrackingConfiguration : IEntityTypeConfiguration<UserProductTracking>
{
    public void Configure(EntityTypeBuilder<UserProductTracking> builder)
    {
        builder.ToTable("UserProductTrackings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.TargetPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.NotifyOnAnyPriceChange)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.NotifyOnAvailability)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.NotificationCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(x => x.User)
            .WithMany(u => u.TrackedProducts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.ProductId);

        builder.HasIndex(x => new { x.UserId, x.ProductId, x.IsActive })
            .HasDatabaseName("IX_UserProductTrackings_UserId_ProductId_IsActive");

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.LastNotifiedAt);
    }
}
