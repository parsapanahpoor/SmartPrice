using SmartPrice.Application.DTOs.Scraper;
using SmartPrice.Domain.Entities;
using SmartPrice.Domain.Enums;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Service for managing the scraping queue
/// </summary>
public interface IScrapingQueueService
{
    /// <summary>
    /// Add a URL to the scraping queue
    /// </summary>
    /// <param name="url">URL to scrape</param>
    /// <param name="jobId">Parent job identifier</param>
    /// <param name="priority">Priority level</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Queue item identifier</returns>
    Task<Guid> EnqueueAsync(string url, Guid jobId, JobPriority priority, CancellationToken ct);

    /// <summary>
    /// Get pending queue items for processing
    /// </summary>
    /// <param name="batchSize">Maximum number of items to retrieve</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of pending queue items</returns>
    Task<List<ScrapingQueue>> GetPendingItemsAsync(int batchSize, CancellationToken ct);

    /// <summary>
    /// Mark a queue item as currently being processed
    /// </summary>
    /// <param name="queueId">Queue item identifier</param>
    /// <param name="ct">Cancellation token</param>
    Task MarkAsProcessingAsync(Guid queueId, CancellationToken ct);

    /// <summary>
    /// Mark a queue item as successfully completed
    /// </summary>
    /// <param name="queueId">Queue item identifier</param>
    /// <param name="result">Scraping result</param>
    /// <param name="ct">Cancellation token</param>
    Task MarkAsCompletedAsync(Guid queueId, ScrapingResult result, CancellationToken ct);

    /// <summary>
    /// Mark a queue item as failed
    /// </summary>
    /// <param name="queueId">Queue item identifier</param>
    /// <param name="error">Error message</param>
    /// <param name="ct">Cancellation token</param>
    Task MarkAsFailedAsync(Guid queueId, string error, CancellationToken ct);

    /// <summary>
    /// Get the current queue length
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of pending items in queue</returns>
    Task<int> GetQueueLengthAsync(CancellationToken ct);
}
