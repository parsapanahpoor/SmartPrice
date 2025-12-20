using SmartPrice.Domain.Entities;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Repository interface for Product entity operations
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Gets a product by its unique identifier
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>The product if found, null otherwise</returns>
    Task<Product?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all products from the database
    /// </summary>
    /// <returns>Collection of all products</returns>
    Task<IEnumerable<Product>> GetAllAsync();

    /// <summary>
    /// Adds a new product to the database
    /// </summary>
    /// <param name="product">Product to add</param>
    /// <returns>The added product</returns>
    Task<Product> AddAsync(Product product);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="product">Product to update</param>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Deletes a product by its ID
    /// </summary>
    /// <param name="id">Product ID to delete</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a product with the specified URL exists
    /// </summary>
    /// <param name="url">Product URL</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ExistsAsync(string url);
}
