using SmartPrice.Domain.Enums;

namespace SmartPrice.Domain.Entities;

/// <summary>
/// Represents a proxy server configuration for web scraping
/// </summary>
public class ProxyServer
{
    /// <summary>
    /// Unique identifier for the proxy server
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// IP address of the proxy server
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Port number of the proxy server
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Optional username for proxy authentication
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Optional password for proxy authentication
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Current status of the proxy server
    /// </summary>
    public ProxyStatus Status { get; set; }

    /// <summary>
    /// Number of consecutive failures
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Last time the proxy was used
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Last time the proxy status was checked
    /// </summary>
    public DateTime? LastCheckedAt { get; set; }

    /// <summary>
    /// When the proxy was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the proxy was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
