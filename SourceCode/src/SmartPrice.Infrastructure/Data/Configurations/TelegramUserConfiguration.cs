using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for TelegramUser entity
/// </summary>
public class TelegramUserConfiguration : IEntityTypeConfiguration<TelegramUser>
{
    public void Configure(EntityTypeBuilder<TelegramUser> builder)
    {
        builder.ToTable("TelegramUsers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChatId)
            .IsRequired();

        builder.Property(x => x.Username)
            .HasMaxLength(100);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.NotificationsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.LanguageCode)
            .HasMaxLength(10)
            .HasDefaultValue("fa");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(x => x.ChatId)
            .IsUnique();

        builder.HasIndex(x => x.Username);

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.LastInteractionAt);
    }
}
