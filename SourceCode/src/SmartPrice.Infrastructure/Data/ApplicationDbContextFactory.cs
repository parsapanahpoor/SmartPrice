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
        
        // Connection string for design-time migrations
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=smartprice;Username=postgres;Password=postgres123",
            b => b.MigrationsAssembly("SmartPrice.Infrastructure")
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
