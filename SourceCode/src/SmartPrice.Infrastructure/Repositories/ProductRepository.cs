using Microsoft.EntityFrameworkCore;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Infrastructure.Data;

namespace SmartPrice.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Product entity
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.PriceHistory)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.PriceHistory)
            .ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.LastUpdated = DateTime.UtcNow;

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        product.LastUpdated = DateTime.UtcNow;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string url)
    {
        return await _context.Products
            .AnyAsync(p => p.Url == url);
    }
}
