using System.Net;
using Microsoft.Extensions.Options;
using SmartPrice.Application.Interfaces;

namespace SmartPrice.Infrastructure.Scraping;

/// <summary>
/// Manages proxy server rotation and health tracking
/// </summary>
public class ProxyManager : IProxyManager
{
    private readonly List<ProxyConfig> _proxies;
    private int _currentIndex = 0;
    private readonly object _lock = new();

    public ProxyManager(IOptions<ScraperOptions> options)
    {
        _proxies = options.Value.Proxies ?? new List<ProxyConfig>();
    }

    /// <summary>
    /// Get the next available proxy from the pool using round-robin
    /// </summary>
    public Task<WebProxy?> GetNextProxyAsync()
    {
        if (!_proxies.Any())
            return Task.FromResult<WebProxy?>(null);

        lock (_lock)
        {
            var proxy = _proxies[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _proxies.Count;

            var webProxy = new WebProxy($"{proxy.Host}:{proxy.Port}");

            if (!string.IsNullOrEmpty(proxy.Username))
            {
                webProxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            return Task.FromResult<WebProxy?>(webProxy);
        }
    }

    /// <summary>
    /// Mark a proxy as failed (for future tracking implementation)
    /// </summary>
    public Task MarkProxyAsFailedAsync(string proxyAddress)
    {
        // TODO: Implement proxy failure tracking with database
        // For now, just return completed task
        return Task.CompletedTask;
    }

    /// <summary>
    /// Mark a proxy as successful (for future tracking implementation)
    /// </summary>
    public Task MarkProxyAsSuccessAsync(string proxyAddress)
    {
        // TODO: Implement proxy success tracking with database
        return Task.CompletedTask;
    }
}
