using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for NotificationLog entity
/// </summary>
public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ProductId);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.IsSent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(x => x.RetryCount)
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
            .WithMany(u => u.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.ProductId);

        builder.HasIndex(x => x.Type);

        builder.HasIndex(x => x.IsSent);

        builder.HasIndex(x => x.SentAt);

        builder.HasIndex(x => new { x.UserId, x.SentAt })
            .HasDatabaseName("IX_NotificationLogs_UserId_SentAt");
    }
}
