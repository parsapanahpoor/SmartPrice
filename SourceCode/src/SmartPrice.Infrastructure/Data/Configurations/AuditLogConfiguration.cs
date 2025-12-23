using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// پیکربندی موجودیت AuditLog
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdminUserId)
            .IsRequired();

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Details)
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(x => x.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(x => x.AdminUserId);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.EntityType);

        // Relations
        builder.HasOne(x => x.AdminUser)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.AdminUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
