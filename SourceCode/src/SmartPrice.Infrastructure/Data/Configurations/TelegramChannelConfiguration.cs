using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for TelegramChannel entity
/// </summary>
public class TelegramChannelConfiguration : IEntityTypeConfiguration<TelegramChannel>
{
    public void Configure(EntityTypeBuilder<TelegramChannel> builder)
    {
        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.ChannelId)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(tc => tc.ChannelId)
            .IsUnique();

        builder.Property(tc => tc.ChannelName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(tc => tc.IsActive)
            .HasDefaultValue(true);

        builder.Property(tc => tc.CreatedAt)
            .IsRequired();
    }
}
