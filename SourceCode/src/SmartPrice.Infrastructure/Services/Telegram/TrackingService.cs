using Microsoft.Extensions.Logging;
using SmartPrice.Application.DTOs.Telegram;
using SmartPrice.Application.Interfaces;
using SmartPrice.Application.Interfaces.Telegram;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Services.Telegram;

/// <summary>
/// Service for managing product tracking
/// </summary>
public class TrackingService : ITrackingService
{
    private readonly IRepository<UserProductTracking> _trackingRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IScraperService _scraperService;
    private readonly ILogger<TrackingService> _logger;

    public TrackingService(
        IRepository<UserProductTracking> trackingRepository,
        IRepository<Product> productRepository,
        IScraperService scraperService,
        ILogger<TrackingService> logger)
    {
        _trackingRepository = trackingRepository;
        _productRepository = productRepository;
        _scraperService = scraperService;
        _logger = logger;
    }

    public async Task<Guid> TrackProductAsync(Guid userId, string productUrl, decimal? targetPrice, CancellationToken ct)
    {
        // Validate URL
        var isValid = await _scraperService.IsUrlValidAsync(productUrl);
        if (!isValid)
        {
            throw new ArgumentException("Invalid product URL", nameof(productUrl));
        }

        // Scrape product to get details
        _logger.LogInformation("Scraping product for tracking: {Url}", productUrl);
        var scrapingResult = await _scraperService.ScrapeProductAsync(productUrl, ct);

        if (!scrapingResult.Success || scrapingResult.Product == null)
        {
            throw new InvalidOperationException($"Failed to scrape product: {scrapingResult.ErrorMessage}");
        }

        // Find or create product in database
        var product = await _productRepository.FirstOrDefaultAsync(
            p => p.Url == productUrl,
            ct);

        if (product == null)
        {
            product = new Product
            {
                Id = Guid.NewGuid(),
                Name = scrapingResult.Product.Title,
                Url = productUrl,
                CurrentPrice = scrapingResult.Product.Price,
                ImageUrl = scrapingResult.Product.ImageUrl ?? string.Empty,
                IsAvailable = scrapingResult.Product.IsAvailable,
                Category = string.Empty,
                DiscountPercentage = 0,
                LastUpdated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product, ct);
            _logger.LogInformation("Created new product: {ProductId} - {Title}", product.Id, product.Name);
        }
        else
        {
            // Update product info
            product.Name = scrapingResult.Product.Title;
            product.CurrentPrice = scrapingResult.Product.Price;
            product.IsAvailable = scrapingResult.Product.IsAvailable;
            product.LastUpdated = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product, ct);
        }

        // Check if already tracking
        var existingTracking = await _trackingRepository.FirstOrDefaultAsync(
            t => t.UserId == userId && t.ProductId == product.Id && t.IsActive,
            ct);

        if (existingTracking != null)
        {
            // Update existing tracking
            existingTracking.TargetPrice = targetPrice;
            existingTracking.UpdatedAt = DateTime.UtcNow;
            await _trackingRepository.UpdateAsync(existingTracking, ct);
            return existingTracking.Id;
        }

        // Create new tracking
        var tracking = new UserProductTracking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = product.Id,
            TargetPrice = targetPrice,
            NotifyOnAnyPriceChange = targetPrice == null,
            NotifyOnAvailability = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _trackingRepository.AddAsync(tracking, ct);

        _logger.LogInformation("User {UserId} started tracking product {ProductId}",
            userId, product.Id);

        return tracking.Id;
    }

    public async Task<bool> UntrackProductAsync(Guid userId, Guid productId, CancellationToken ct)
    {
        var tracking = await _trackingRepository.FirstOrDefaultAsync(
            t => t.UserId == userId && t.ProductId == productId && t.IsActive,
            ct);

        if (tracking == null)
        {
            return false;
        }

        tracking.IsActive = false;
        tracking.UpdatedAt = DateTime.UtcNow;
        await _trackingRepository.UpdateAsync(tracking, ct);

        _logger.LogInformation("User {UserId} stopped tracking product {ProductId}",
            userId, productId);

        return true;
    }

    public async Task<List<UserProductTrackingDto>> GetUserTrackedProductsAsync(Guid userId, CancellationToken ct)
    {
        var trackings = await _trackingRepository.FindAsync(
            t => t.UserId == userId && t.IsActive,
            ct);

        var result = new List<UserProductTrackingDto>();

        foreach (var tracking in trackings)
        {
            var product = await _productRepository.GetByIdAsync(tracking.ProductId, ct);
            if (product == null) continue;

            result.Add(new UserProductTrackingDto
            {
                Id = tracking.Id,
                ProductTitle = product.Name,
                ProductUrl = product.Url,
                CurrentPrice = product.CurrentPrice,
                TargetPrice = tracking.TargetPrice,
                IsAvailable = product.IsAvailable,
                TrackedSince = tracking.CreatedAt,
                LastNotifiedAt = tracking.LastNotifiedAt,
                ImageUrl = product.ImageUrl,
                NotificationCount = tracking.NotificationCount
            });
        }

        return result;
    }

    public async Task<bool> UpdateTargetPriceAsync(Guid trackingId, decimal targetPrice, CancellationToken ct)
    {
        var tracking = await _trackingRepository.GetByIdAsync(trackingId, ct);
        if (tracking == null)
        {
            return false;
        }

        tracking.TargetPrice = targetPrice;
        tracking.NotifyOnAnyPriceChange = false;
        tracking.UpdatedAt = DateTime.UtcNow;
        await _trackingRepository.UpdateAsync(tracking, ct);

        return true;
    }

    public async Task<List<UserProductTracking>> GetUsersToNotifyAsync(Guid productId, CancellationToken ct)
    {
        var trackings = await _trackingRepository.FindAsync(
            t => t.ProductId == productId && t.IsActive,
            ct);

        // Load related entities
        var result = new List<UserProductTracking>();
        foreach (var tracking in trackings)
        {
            var product = await _productRepository.GetByIdAsync(tracking.ProductId, ct);
            if (product != null)
            {
                tracking.Product = product;
                result.Add(tracking);
            }
        }

        return result;
    }

    public async Task<bool> IsTrackingProductAsync(Guid userId, string productUrl, CancellationToken ct)
    {
        var product = await _productRepository.FirstOrDefaultAsync(
            p => p.Url == productUrl,
            ct);

        if (product == null)
        {
            return false;
        }

        var tracking = await _trackingRepository.FirstOrDefaultAsync(
            t => t.UserId == userId && t.ProductId == product.Id && t.IsActive,
            ct);

        return tracking != null;
    }
}
