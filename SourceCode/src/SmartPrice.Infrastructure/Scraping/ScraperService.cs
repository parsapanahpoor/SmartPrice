using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Scraping;

/// <summary>
/// Core service for orchestrating web scraping operations
/// </summary>
public class ScraperService : IScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProxyManager _proxyManager;
    private readonly IEnumerable<IMarketplaceScraper> _scrapers;
    private readonly ILogger<ScraperService> _logger;
    private readonly ScraperOptions _options;
    private readonly SemaphoreSlim _semaphore;

    public ScraperService(
        IHttpClientFactory httpClientFactory,
        IProxyManager proxyManager,
        IEnumerable<IMarketplaceScraper> scrapers,
        ILogger<ScraperService> logger,
        IOptions<ScraperOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _proxyManager = proxyManager;
        _scrapers = scrapers;
        _logger = logger;
        _options = options.Value;
        _semaphore = new SemaphoreSlim(_options.MaxConcurrentRequests);
    }

    /// <summary>
    /// Scrape a single product URL with rate limiting and error handling
    /// </summary>
    public async Task<ScrapingResult> ScrapeProductAsync(string url, CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var marketplace = DetectMarketplace(url);
            var scraper = _scrapers.FirstOrDefault(s => s.CanHandle(url));

            if (scraper == null)
            {
                _logger.LogWarning("No scraper available for URL: {Url}", url);
                return new ScrapingResult
                {
                    Success = false,
                    Url = url,
                    ErrorMessage = "No scraper available for this URL",
                    Marketplace = marketplace
                };
            }

            _logger.LogInformation("Starting scrape for {Url} using {Marketplace}", url, marketplace);

            var sw = Stopwatch.StartNew();
            var result = await scraper.ScrapeAsync(url, ct);
            sw.Stop();

            result.Duration = sw.Elapsed;
            result.Marketplace = marketplace;

            if (result.Success)
            {
                _logger.LogInformation("Successfully scraped {Url} in {Duration}ms", 
                    url, sw.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning("Failed to scrape {Url}: {Error}", 
                    url, result.ErrorMessage);
            }

            // Add delay between requests to be polite
            await Task.Delay(_options.RequestDelayMs, ct);

            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Scrape multiple products concurrently
    /// </summary>
    public async Task<List<ScrapingResult>> ScrapeProductsAsync(List<string> urls, CancellationToken ct)
    {
        _logger.LogInformation("Starting batch scrape for {Count} URLs", urls.Count);

        var tasks = urls.Select(url => ScrapeProductAsync(url, ct));
        var results = await Task.WhenAll(tasks);

        var successCount = results.Count(r => r.Success);
        _logger.LogInformation("Batch scrape completed: {Success}/{Total} successful", 
            successCount, urls.Count);

        return results.ToList();
    }

    /// <summary>
    /// Detect marketplace type from URL
    /// </summary>
    public MarketplaceType DetectMarketplace(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return MarketplaceType.Other;

        var lowerUrl = url.ToLowerInvariant();

        if (lowerUrl.Contains("digikala.com"))
            return MarketplaceType.Digikala;

        if (lowerUrl.Contains("torob.com"))
            return MarketplaceType.Torob;

        if (lowerUrl.Contains("snappfood.ir") || lowerUrl.Contains("snapfood.ir"))
            return MarketplaceType.Snapfood;

        if (lowerUrl.Contains("emalls.ir"))
            return MarketplaceType.Emalls;

        return MarketplaceType.Other;
    }

    /// <summary>
    /// Validate URL format
    /// </summary>
    public Task<bool> IsUrlValidAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult(false);

        var isValid = Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        return Task.FromResult(isValid);
    }
}
