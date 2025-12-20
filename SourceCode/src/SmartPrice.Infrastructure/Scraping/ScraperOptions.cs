namespace SmartPrice.Infrastructure.Scraping;

/// <summary>
/// Configuration options for the web scraper
/// </summary>
public class ScraperOptions
{
    /// <summary>
    /// Maximum number of concurrent scraping requests
    /// </summary>
    public int MaxConcurrentRequests { get; set; } = 5;

    /// <summary>
    /// Delay between requests in milliseconds
    /// </summary>
    public int RequestDelayMs { get; set; } = 2000;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Whether to use proxy servers
    /// </summary>
    public bool UseProxy { get; set; } = false;

    /// <summary>
    /// List of user agents to rotate
    /// </summary>
    public List<string> UserAgents { get; set; } = new();

    /// <summary>
    /// List of proxy configurations
    /// </summary>
    public List<ProxyConfig> Proxies { get; set; } = new();
}

/// <summary>
/// Configuration for a proxy server
/// </summary>
public class ProxyConfig
{
    /// <summary>
    /// Proxy host address
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Proxy port number
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Optional proxy username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Optional proxy password
    /// </summary>
    public string? Password { get; set; }
}
