using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ProxyServer entity
/// </summary>
public class ProxyServerConfiguration : IEntityTypeConfiguration<ProxyServer>
{
    public void Configure(EntityTypeBuilder<ProxyServer> builder)
    {
        builder.ToTable("ProxyServers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // Max length for IPv6

        builder.Property(p => p.Port)
            .IsRequired();

        builder.Property(p => p.Username)
            .HasMaxLength(100);

        builder.Property(p => p.Password)
            .HasMaxLength(200);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.FailureCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.LastUsedAt)
            .IsRequired(false);

        builder.Property(p => p.LastCheckedAt)
            .IsRequired(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Index for quick status lookups
        builder.HasIndex(p => p.Status);

        // Composite index for IP and Port
        builder.HasIndex(p => new { p.IpAddress, p.Port })
            .IsUnique();
    }
}
