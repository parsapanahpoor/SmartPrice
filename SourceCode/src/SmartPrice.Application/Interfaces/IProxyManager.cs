using System.Net;

namespace SmartPrice.Application.Interfaces;

/// <summary>
/// Service for managing proxy servers
/// </summary>
public interface IProxyManager
{
    /// <summary>
    /// Get the next available proxy from the pool
    /// </summary>
    /// <returns>A WebProxy instance or null if no proxies are configured</returns>
    Task<WebProxy?> GetNextProxyAsync();

    /// <summary>
    /// Mark a proxy as failed to track reliability
    /// </summary>
    /// <param name="proxyAddress">The proxy address that failed</param>
    Task MarkProxyAsFailedAsync(string proxyAddress);

    /// <summary>
    /// Mark a proxy as successful to track reliability
    /// </summary>
    /// <param name="proxyAddress">The proxy address that succeeded</param>
    Task MarkProxyAsSuccessAsync(string proxyAddress);
}
