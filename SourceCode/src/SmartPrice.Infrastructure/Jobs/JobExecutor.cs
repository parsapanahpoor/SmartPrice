using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Jobs;

/// <summary>
/// Service for executing scraping jobs
/// </summary>
public class JobExecutor : IJobExecutor
{
    private readonly IScraperService _scraperService;
    private readonly IScrapingQueueService _queueService;
    private readonly IRepository<ScrapingJob> _jobRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<PriceHistory> _priceHistoryRepository;
    private readonly ILogger<JobExecutor> _logger;

    public JobExecutor(
        IScraperService scraperService,
        IScrapingQueueService queueService,
        IRepository<ScrapingJob> jobRepository,
        IRepository<Product> productRepository,
        IRepository<PriceHistory> priceHistoryRepository,
        ILogger<JobExecutor> logger)
    {
        _scraperService = scraperService;
        _queueService = queueService;
        _jobRepository = jobRepository;
        _productRepository = productRepository;
        _priceHistoryRepository = priceHistoryRepository;
        _logger = logger;
    }

    public async Task ExecuteJobAsync(Guid jobId, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var job = await _jobRepository.GetByIdAsync(jobId, ct);

        if (job == null)
        {
            _logger.LogWarning("Job {JobId} not found", jobId);
            return;
        }

        _logger.LogInformation("Starting job execution: {JobName} ({JobId})", job.Name, jobId);

        // Update job status
        job.Status = JobStatus.Running;
        job.StartedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        await _jobRepository.UpdateAsync(job, ct);

        var results = new JobExecutionResult
        {
            Success = true
        };

        try
        {
            // Get pending items from queue
            var pendingItems = await _queueService.GetPendingItemsAsync(100, ct);

            _logger.LogInformation("Processing {Count} URLs for job {JobName}",
                pendingItems.Count, job.Name);

            foreach (var item in pendingItems)
            {
                if (ct.IsCancellationRequested)
                {
                    _logger.LogWarning("Job execution cancelled for {JobName}", job.Name);
                    break;
                }

                var result = await ExecuteUrlAsync(item.Url, jobId, ct);
                results.ProcessedCount += result.ProcessedCount;
                results.FailedCount += result.FailedCount;
                results.Errors.AddRange(result.Errors);
            }

            sw.Stop();
            results.Duration = sw.Elapsed;

            // Update job with results
            job.Status = JobStatus.Completed;
            job.SuccessCount += results.ProcessedCount;
            job.FailureCount += results.FailedCount;
            job.ProductsScraped = results.ProcessedCount;
            job.CompletedAt = DateTime.UtcNow;
            job.Duration = sw.Elapsed;
            job.UpdatedAt = DateTime.UtcNow;

            if (results.FailedCount > 0)
            {
                job.ErrorMessage = $"{results.FailedCount} items failed. Errors: {string.Join(", ", results.Errors.Take(3))}";
            }

            _logger.LogInformation(
                "Job completed: {JobName}. Processed: {Processed}, Failed: {Failed}, Duration: {Duration}ms",
                job.Name, results.ProcessedCount, results.FailedCount, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing job {JobName} ({JobId})", job.Name, jobId);

            job.Status = JobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            results.Success = false;
        }

        await _jobRepository.UpdateAsync(job, ct);
    }

    public async Task<JobExecutionResult> ExecuteUrlAsync(string url, Guid jobId, CancellationToken ct)
    {
        var result = new JobExecutionResult
        {
            Success = true
        };

        try
        {
            // Get the specific queue item for this URL
            var queueItems = await _queueService.GetPendingItemsAsync(1, ct);
            var item = queueItems.FirstOrDefault(q => q.Url == url && q.ScrapingJobId == jobId);

            if (item == null)
            {
                _logger.LogWarning("Queue item not found for URL: {Url} and Job: {JobId}", url, jobId);
                return result;
            }

            // Mark as processing
            await _queueService.MarkAsProcessingAsync(item.Id, ct);

            _logger.LogDebug("Scraping URL: {Url}", url);

            // Scrape the URL
            var scrapingResult = await _scraperService.ScrapeProductAsync(url, ct);

            if (scrapingResult.Success && scrapingResult.Product != null)
            {
                // Save the product to database
                await SaveProductAsync(scrapingResult.Product, url, scrapingResult.Marketplace, ct);

                // Mark queue item as completed
                await _queueService.MarkAsCompletedAsync(item.Id, scrapingResult, ct);

                result.ProcessedCount++;

                _logger.LogInformation("Successfully scraped and saved product from: {Url}", url);
            }
            else
            {
                // Mark queue item as failed
                var errorMessage = scrapingResult.ErrorMessage ?? "Unknown error";
                await _queueService.MarkAsFailedAsync(item.Id, errorMessage, ct);

                result.FailedCount++;
                result.Errors.Add($"{url}: {errorMessage}");

                _logger.LogWarning("Failed to scrape {Url}: {Error}", url, errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing URL: {Url}", url);
            result.FailedCount++;
            result.Errors.Add($"{url}: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    private async Task SaveProductAsync(ScrapedProductDto dto, string url, MarketplaceType marketplace, CancellationToken ct)
    {
        // Try to find existing product by URL or SKU
        var existingProduct = await _productRepository.FirstOrDefaultAsync(
            p => p.Url == url,
            ct);

        if (existingProduct != null)
        {
            // Update existing product
            existingProduct.Name = dto.Title;
            existingProduct.CurrentPrice = dto.Price;
            existingProduct.IsAvailable = dto.IsAvailable;
            existingProduct.ImageUrl = dto.ImageUrl ?? existingProduct.ImageUrl;
            existingProduct.LastUpdated = DateTime.UtcNow;

            await _productRepository.UpdateAsync(existingProduct, ct);

            _logger.LogDebug("Updated existing product: {ProductName}", dto.Title);

            // Add price history if price changed
            var lastPrice = await _priceHistoryRepository.FirstOrDefaultAsync(
                ph => ph.ProductId == existingProduct.Id,
                ct);

            if (lastPrice == null || lastPrice.Price != dto.Price)
            {
                var priceHistory = new PriceHistory
                {
                    Id = Guid.NewGuid(),
                    ProductId = existingProduct.Id,
                    Price = dto.Price,
                    RecordedAt = DateTime.UtcNow
                };

                await _priceHistoryRepository.AddAsync(priceHistory, ct);

                _logger.LogDebug("Added price history for product: {ProductName}. Old: {OldPrice}, New: {NewPrice}",
                    dto.Title, lastPrice?.Price, dto.Price);
            }
        }
        else
        {
            // Create new product
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Title,
                Url = url,
                CurrentPrice = dto.Price,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                IsAvailable = dto.IsAvailable,
                Category = string.Empty, // Could be extracted from metadata
                DiscountPercentage = 0,
                OriginalPrice = null,
                LastUpdated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product, ct);

            _logger.LogInformation("Created new product: {ProductName} from {Marketplace}",
                dto.Title, marketplace);

            // Add initial price history
            var priceHistory = new PriceHistory
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Price = dto.Price,
                RecordedAt = DateTime.UtcNow
            };

            await _priceHistoryRepository.AddAsync(priceHistory, ct);
        }
    }
}
