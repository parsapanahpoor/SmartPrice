using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartPrice.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating ApplicationDbContext during migrations
/// This factory is used only during design-time operations like migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use a default connection string for migrations
        // This will be overridden at runtime by the actual configuration
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=smartprice;Username=postgres;Password=your_password",
            b => b.MigrationsAssembly("SmartPrice.Infrastructure")
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
