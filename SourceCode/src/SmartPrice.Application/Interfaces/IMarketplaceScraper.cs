using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Interface for marketplace-specific scraper implementations
/// </summary>
public interface IMarketplaceScraper
{
    /// <summary>
    /// The marketplace type this scraper handles
    /// </summary>
    MarketplaceType Marketplace { get; }

    /// <summary>
    /// Scrape a product from a URL
    /// </summary>
    /// <param name="url">The product URL to scrape</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Scraping result with product data</returns>
    Task<ScrapingResult> ScrapeAsync(string url, CancellationToken ct);

    /// <summary>
    /// Check if this scraper can handle the given URL
    /// </summary>
    /// <param name="url">The URL to check</param>
    /// <returns>True if this scraper can handle the URL</returns>
    bool CanHandle(string url);
}
