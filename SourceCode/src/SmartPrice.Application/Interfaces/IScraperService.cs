using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Service for orchestrating web scraping operations
/// </summary>
public interface IScraperService
{
    /// <summary>
    /// Scrape a single product from a URL
    /// </summary>
    /// <param name="url">The product URL to scrape</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Scraping result with product data or error information</returns>
    Task<ScrapingResult> ScrapeProductAsync(string url, CancellationToken ct);

    /// <summary>
    /// Scrape multiple products from a list of URLs
    /// </summary>
    /// <param name="urls">List of product URLs to scrape</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of scraping results</returns>
    Task<List<ScrapingResult>> ScrapeProductsAsync(List<string> urls, CancellationToken ct);

    /// <summary>
    /// Validate if a URL is properly formatted
    /// </summary>
    /// <param name="url">The URL to validate</param>
    /// <returns>True if URL is valid, false otherwise</returns>
    Task<bool> IsUrlValidAsync(string url);

    /// <summary>
    /// Detect which marketplace a URL belongs to
    /// </summary>
    /// <param name="url">The URL to analyze</param>
    /// <returns>The detected marketplace type</returns>
    MarketplaceType DetectMarketplace(string url);
}
