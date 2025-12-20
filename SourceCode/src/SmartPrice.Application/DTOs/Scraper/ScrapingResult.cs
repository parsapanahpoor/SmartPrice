using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.DTOs.Scraper;

/// <summary>
/// Result of a scraping operation
/// </summary>
public class ScrapingResult
{
    /// <summary>
    /// Indicates if the scraping was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The URL that was scraped
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Scraped product data if successful
    /// </summary>
    public ScrapedProductDto? Product { get; set; }

    /// <summary>
    /// Error message if scraping failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Time taken to complete the scraping
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Type of marketplace scraped
    /// </summary>
    public MarketplaceType Marketplace { get; set; }
}

/// <summary>
/// Scraped product data transfer object
/// </summary>
public class ScrapedProductDto
{
    /// <summary>
    /// Product title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Product image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Whether the product is available for purchase
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Product SKU or unique identifier
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata about the product
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
