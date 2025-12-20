namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a historical price record for a product
/// </summary>
public class PriceHistory
{
    /// <summary>
    /// Unique identifier for the price history record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the product
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The price at this point in time
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// When this price was recorded
    /// </summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>
    /// Navigation property to the product
    /// </summary>
    public Product Product { get; set; } = null!;
}
