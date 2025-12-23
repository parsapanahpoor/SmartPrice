using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;
using SmartPrice.Infrastructure.Data;

namespace SmartPrice.Infrastructure.Data;

/// <summary>
/// کلاس برای seed کردن داده‌های اولیه
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seed کردن داده‌های اولیه
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // اطمینان از وجود Database
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrated successfully");

            // Seed Admin User
            await SeedAdminUserAsync(context, logger);

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }

    /// <summary>
    /// ایجاد کاربر ادمین اولیه
    /// </summary>
    private static async Task SeedAdminUserAsync(ApplicationDbContext context, ILogger logger)
    {
        // بررسی اینکه آیا کاربر ادمین وجود دارد یا نه
        if (await context.AdminUsers.AnyAsync())
        {
            logger.LogInformation("Admin users already exist, skipping seed");
            return;
        }

        var adminUser = new AdminUser
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@smartprice.ir",
            FullName = "System Administrator",
            Role = AdminRole.SuperAdmin,
            IsActive = true,
            // Password: Admin@123
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.AdminUsers.AddAsync(adminUser);
        await context.SaveChangesAsync();

        logger.LogInformation("Default admin user created: {Username}", adminUser.Username);
        logger.LogInformation("Default admin password: Admin@123");
        logger.LogWarning("⚠️  IMPORTANT: Change the default admin password after first login!");
    }
}
