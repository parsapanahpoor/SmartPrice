using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Application.Interfaces;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Infrastructure.Jobs;

/// <summary>
/// Service for managing the scraping queue
/// </summary>
public class ScrapingQueueService : IScrapingQueueService
{
    private readonly IRepository<ScrapingQueue> _queueRepository;
    private readonly IScraperService _scraperService;
    private readonly ILogger<ScrapingQueueService> _logger;

    public ScrapingQueueService(
        IRepository<ScrapingQueue> queueRepository,
        IScraperService scraperService,
        ILogger<ScrapingQueueService> logger)
    {
        _queueRepository = queueRepository;
        _scraperService = scraperService;
        _logger = logger;
    }

    public async Task<Guid> EnqueueAsync(string url, Guid jobId, JobPriority priority, CancellationToken ct)
    {
        var marketplace = _scraperService.DetectMarketplace(url);

        var item = new ScrapingQueue
        {
            Id = Guid.NewGuid(),
            ScrapingJobId = jobId,
            Url = url,
            Priority = priority,
            Status = ScrapingStatus.Pending,
            ScheduledAt = DateTime.UtcNow,
            Marketplace = marketplace,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _queueRepository.AddAsync(item, ct);

        _logger.LogInformation("URL enqueued: {Url} for Job {JobId} with priority {Priority}",
            url, jobId, priority);

        return item.Id;
    }

    public async Task<List<ScrapingQueue>> GetPendingItemsAsync(int batchSize, CancellationToken ct)
    {
        var allPending = await _queueRepository.FindAsync(
            q => q.Status == ScrapingStatus.Pending,
            ct);

        // Order by priority (highest first) then by scheduled date (earliest first)
        var orderedItems = allPending
            .OrderByDescending(q => q.Priority)
            .ThenBy(q => q.ScheduledAt)
            .Take(batchSize)
            .ToList();

        _logger.LogDebug("Retrieved {Count} pending items from queue (batch size: {BatchSize})",
            orderedItems.Count, batchSize);

        return orderedItems;
    }

    public async Task MarkAsProcessingAsync(Guid queueId, CancellationToken ct)
    {
        var item = await _queueRepository.GetByIdAsync(queueId, ct);
        if (item == null)
        {
            _logger.LogWarning("Queue item {QueueId} not found", queueId);
            return;
        }

        item.Status = ScrapingStatus.InProgress;
        item.ProcessedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        await _queueRepository.UpdateAsync(item, ct);

        _logger.LogDebug("Queue item {QueueId} marked as processing", queueId);
    }

    public async Task MarkAsCompletedAsync(Guid queueId, ScrapingResult result, CancellationToken ct)
    {
        var item = await _queueRepository.GetByIdAsync(queueId, ct);
        if (item == null)
        {
            _logger.LogWarning("Queue item {QueueId} not found", queueId);
            return;
        }

        item.Status = ScrapingStatus.Completed;
        item.Result = JsonSerializer.Serialize(result);
        item.UpdatedAt = DateTime.UtcNow;

        await _queueRepository.UpdateAsync(item, ct);

        _logger.LogInformation("Queue item {QueueId} completed successfully for URL: {Url}",
            queueId, item.Url);
    }

    public async Task MarkAsFailedAsync(Guid queueId, string error, CancellationToken ct)
    {
        var item = await _queueRepository.GetByIdAsync(queueId, ct);
        if (item == null)
        {
            _logger.LogWarning("Queue item {QueueId} not found", queueId);
            return;
        }

        item.Status = ScrapingStatus.Failed;
        item.ErrorMessage = error;
        item.RetryCount++;
        item.UpdatedAt = DateTime.UtcNow;

        await _queueRepository.UpdateAsync(item, ct);

        _logger.LogWarning("Queue item {QueueId} failed: {Error}. Retry count: {RetryCount}",
            queueId, error, item.RetryCount);
    }

    public async Task<int> GetQueueLengthAsync(CancellationToken ct)
    {
        var count = await _queueRepository.CountAsync(
            q => q.Status == ScrapingStatus.Pending,
            ct);

        return count;
    }
}
