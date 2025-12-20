using SmartPrice.Application.DTOs;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Interface for web scraping functionality
/// </summary>
public interface IScraper
{
    /// <summary>
    /// Scrapes products from the specified URL
    /// </summary>
    /// <param name="url">The URL to scrape</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of scraped products</returns>
    Task<IEnumerable<ProductDto>> ScrapeProductsAsync(
        string url,
        CancellationToken cancellationToken = default
    );
}
