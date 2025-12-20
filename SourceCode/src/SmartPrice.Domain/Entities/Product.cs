namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a product tracked in the system
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product URL from the source website
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Product image URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Product category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Current price of the product
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Original price before discount (if applicable)
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// Discount percentage (0-100)
    /// </summary>
    public int DiscountPercentage { get; set; }

    /// <summary>
    /// Whether the product is currently available for purchase
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Last time the product information was updated
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// When the product was first added to the system
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Historical price records for this product
    /// </summary>
    public ICollection<PriceHistory> PriceHistory { get; set; } = new List<PriceHistory>();
}
