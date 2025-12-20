namespace SmartPrice.Application.DTOs;

/// <summary>
/// Data transfer object for product information
/// </summary>
/// <param name="Name">Product name</param>
/// <param name="Url">Product URL</param>
/// <param name="ImageUrl">Product image URL</param>
/// <param name="Category">Product category</param>
/// <param name="CurrentPrice">Current price</param>
/// <param name="OriginalPrice">Original price before discount</param>
/// <param name="DiscountPercentage">Discount percentage (0-100)</param>
/// <param name="IsAvailable">Whether the product is available</param>
public record ProductDto(
    string Name,
    string Url,
    string ImageUrl,
    string Category,
    decimal CurrentPrice,
    decimal? OriginalPrice,
    int DiscountPercentage,
    bool IsAvailable
);
