using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Scraping.Scrapers;

/// <summary>
/// Scraper implementation for Digikala marketplace
/// </summary>
public class DigikalaScraper : IMarketplaceScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProxyManager _proxyManager;
    private readonly ILogger<DigikalaScraper> _logger;
    private readonly ScraperOptions _options;
    private static readonly Random _random = new();

    public MarketplaceType Marketplace => MarketplaceType.Digikala;

    public DigikalaScraper(
        IHttpClientFactory httpClientFactory,
        IProxyManager proxyManager,
        ILogger<DigikalaScraper> logger,
        IOptions<ScraperOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _proxyManager = proxyManager;
        _logger = logger;
        _options = options.Value;
    }

    /// <summary>
    /// Check if this scraper can handle the URL
    /// </summary>
    public bool CanHandle(string url)
    {
        return !string.IsNullOrWhiteSpace(url) && url.Contains("digikala.com", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Scrape product data from Digikala with retry logic
    /// </summary>
    public async Task<ScrapingResult> ScrapeAsync(string url, CancellationToken ct)
    {
        var retries = 0;
        Exception? lastException = null;

        while (retries < _options.MaxRetries)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("ScraperClient");
                client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

                // Add random User-Agent to avoid detection
                if (_options.UserAgents.Any())
                {
                    var userAgent = _options.UserAgents[_random.Next(_options.UserAgents.Count)];
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                    client.DefaultRequestHeaders.Add("Accept-Language", "fa-IR,fa;q=0.9,en-US;q=0.8,en;q=0.7");
                }

                _logger.LogDebug("Fetching Digikala URL: {Url}, Attempt {Retry}/{Max}", 
                    url, retries + 1, _options.MaxRetries);

                var response = await client.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync(ct);

                if (string.IsNullOrWhiteSpace(html))
                {
                    throw new InvalidOperationException("Received empty HTML response");
                }

                var product = ParseDigikalaHtml(html, url);

                _logger.LogInformation("Successfully parsed Digikala product: {Title}", product.Title);

                return new ScrapingResult
                {
                    Success = true,
                    Url = url,
                    Product = product
                };
            }
            catch (Exception ex)
            {
                lastException = ex;
                retries++;

                _logger.LogWarning(ex, "Scraping failed for {Url}, Retry {Retry}/{Max}", 
                    url, retries, _options.MaxRetries);

                if (retries < _options.MaxRetries)
                {
                    // Exponential backoff
                    var delaySeconds = Math.Pow(2, retries);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct);
                }
            }
        }

        return new ScrapingResult
        {
            Success = false,
            Url = url,
            ErrorMessage = lastException?.Message ?? "Unknown error"
        };
    }

    /// <summary>
    /// Parse HTML content to extract product information
    /// </summary>
    private ScrapedProductDto ParseDigikalaHtml(string html, string url)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Extract product title - multiple possible selectors
        var title = ExtractTitle(doc);

        // Extract price - multiple possible selectors
        var price = ExtractPrice(doc);

        // Extract image URL
        var imageUrl = ExtractImageUrl(doc);

        // Check availability
        var isAvailable = CheckAvailability(doc);

        // Extract SKU from URL
        var sku = ExtractSkuFromUrl(url);

        _logger.LogDebug("Parsed product - Title: {Title}, Price: {Price}, Available: {Available}", 
            title, price, isAvailable);

        return new ScrapedProductDto
        {
            Title = title,
            Price = price,
            ImageUrl = imageUrl,
            IsAvailable = isAvailable,
            Sku = sku,
            Metadata = new Dictionary<string, string>
            {
                { "Source", "Digikala" },
                { "ScrapedAt", DateTime.UtcNow.ToString("O") }
            }
        };
    }

    private string ExtractTitle(HtmlDocument doc)
    {
        // Try multiple selectors for title
        var titleSelectors = new[]
        {
            "//h1[@class='text-h4 font-bold']",
            "//h1[contains(@class, 'text-h')]",
            "//h1",
            "//meta[@property='og:title']/@content",
            "//title"
        };

        foreach (var selector in titleSelectors)
        {
            var node = doc.DocumentNode.SelectSingleNode(selector);
            if (node != null)
            {
                var title = selector.Contains("@content") 
                    ? node.GetAttributeValue("content", null) 
                    : node.InnerText;

                if (!string.IsNullOrWhiteSpace(title))
                {
                    return HtmlEntity.DeEntitize(title.Trim());
                }
            }
        }

        _logger.LogWarning("Could not extract title from Digikala page");
        return "Unknown Product";
    }

    private decimal ExtractPrice(HtmlDocument doc)
    {
        // Try multiple selectors for price
        var priceSelectors = new[]
        {
            "//div[contains(@class, 'price-section')]//span[contains(@class, 'text-h5')]",
            "//div[contains(@class, 'price')]//span[contains(@class, 'price-value')]",
            "//span[contains(@data-testid, 'price-final')]",
            "//span[contains(@class, 'price-now')]"
        };

        foreach (var selector in priceSelectors)
        {
            var priceNode = doc.DocumentNode.SelectSingleNode(selector);
            if (priceNode != null)
            {
                var priceText = priceNode.InnerText
                    .Replace(",", "")
                    .Replace("تومان", "")
                    .Replace("ریال", "")
                    .Replace(" ", "")
                    .Trim();

                if (decimal.TryParse(priceText, out var price))
                {
                    return price;
                }
            }
        }

        _logger.LogWarning("Could not extract price from Digikala page");
        return 0;
    }

    private string? ExtractImageUrl(HtmlDocument doc)
    {
        // Try multiple selectors for image
        var imageSelectors = new[]
        {
            "//img[@class='w-full object-contain']/@src",
            "//img[contains(@class, 'product-image')]/@src",
            "//meta[@property='og:image']/@content",
            "//img[contains(@alt, 'تصویر')]/@src"
        };

        foreach (var selector in imageSelectors)
        {
            var imageNode = doc.DocumentNode.SelectSingleNode(selector);
            if (imageNode != null)
            {
                var imageUrl = selector.Contains("@")
                    ? imageNode.GetAttributeValue(selector.Split('@').Last(), null)
                    : imageNode.InnerText;

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    return imageUrl.Trim();
                }
            }
        }

        return null;
    }

    private bool CheckAvailability(HtmlDocument doc)
    {
        // Check for unavailable indicators
        var unavailableIndicators = new[]
        {
            "ناموجود",
            "unavailable",
            "out-of-stock",
            "موجود نیست"
        };

        var htmlContent = doc.DocumentNode.InnerHtml.ToLowerInvariant();

        foreach (var indicator in unavailableIndicators)
        {
            if (htmlContent.Contains(indicator.ToLowerInvariant()))
            {
                return false;
            }
        }

        return true;
    }

    private string ExtractSkuFromUrl(string url)
    {
        // Digikala URLs typically contain product IDs like: dkp-123456
        var match = Regex.Match(url, @"dkp-(\d+)", RegexOptions.IgnoreCase);
        
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Fallback: try to find any number sequence
        match = Regex.Match(url, @"/(\d+)/?");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // Last resort: generate a unique ID
        _logger.LogWarning("Could not extract SKU from URL: {Url}", url);
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
