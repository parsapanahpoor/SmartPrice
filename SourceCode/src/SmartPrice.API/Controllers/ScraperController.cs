using Microsoft.AspNetCore.Mvc;
using SmartPrice.Application.Interfaces;

namespace SmartPrice.API.Controllers;

/// <summary>
/// Web scraper management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ScraperController : ControllerBase
{
    private readonly IScraperService _scraperService;
    private readonly ILogger<ScraperController> _logger;

    public ScraperController(
        IScraperService scraperService,
        ILogger<ScraperController> logger)
    {
        _scraperService = scraperService;
        _logger = logger;
    }

    /// <summary>
    /// Test scraper with a single URL
    /// </summary>
    /// <param name="request">Scraping request with URL</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Scraping result with product data or error</returns>
    /// <response code="200">Returns the scraping result</response>
    /// <response code="400">If the URL is invalid</response>
    [HttpPost("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestScraper(
        [FromBody] ScraperTestRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new { error = "URL is required" });
        }

        var isValid = await _scraperService.IsUrlValidAsync(request.Url);
        if (!isValid)
        {
            return BadRequest(new { error = "Invalid URL format" });
        }

        _logger.LogInformation("Testing scraper for URL: {Url}", request.Url);

        var result = await _scraperService.ScrapeProductAsync(request.Url, ct);

        return Ok(result);
    }

    /// <summary>
    /// Scrape multiple URLs in batch
    /// </summary>
    /// <param name="request">Batch scraping request with multiple URLs</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of scraping results</returns>
    /// <response code="200">Returns the list of scraping results</response>
    /// <response code="400">If no URLs provided</response>
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchScrape(
        [FromBody] BatchScraperRequest request,
        CancellationToken ct)
    {
        if (request.Urls == null || !request.Urls.Any())
        {
            return BadRequest(new { error = "At least one URL is required" });
        }

        _logger.LogInformation("Batch scraping {Count} URLs", request.Urls.Count);

        var results = await _scraperService.ScrapeProductsAsync(request.Urls, ct);

        return Ok(new
        {
            total = results.Count,
            successful = results.Count(r => r.Success),
            failed = results.Count(r => !r.Success),
            results
        });
    }

    /// <summary>
    /// Validate a URL without scraping
    /// </summary>
    /// <param name="request">URL validation request</param>
    /// <returns>Validation result with marketplace detection</returns>
    /// <response code="200">Returns validation result</response>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateUrl([FromBody] ScraperTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return Ok(new
            {
                valid = false,
                marketplace = "Unknown",
                message = "URL is required"
            });
        }

        var isValid = await _scraperService.IsUrlValidAsync(request.Url);
        var marketplace = _scraperService.DetectMarketplace(request.Url);

        return Ok(new
        {
            valid = isValid,
            marketplace = marketplace.ToString(),
            message = isValid ? "URL is valid" : "URL format is invalid"
        });
    }
}

/// <summary>
/// Request model for testing scraper
/// </summary>
public class ScraperTestRequest
{
    /// <summary>
    /// URL to scrape
    /// </summary>
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Request model for batch scraping
/// </summary>
public class BatchScraperRequest
{
    /// <summary>
    /// List of URLs to scrape
    /// </summary>
    public List<string> Urls { get; set; } = new();
}
