namespace SmartPrice.Domain.Enums;

/// <summary>
/// Represents the status of a scraping operation
/// </summary>
public enum ScrapingStatus
{
    /// <summary>
    /// Scraping is waiting to be executed
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Scraping is currently in progress
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Scraping completed successfully
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Scraping failed due to an error
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// Scraping was skipped
    /// </summary>
    Skipped = 4
}

/// <summary>
/// Represents the type of marketplace being scraped
/// </summary>
public enum MarketplaceType
{
    /// <summary>
    /// Digikala marketplace
    /// </summary>
    Digikala = 0,
    
    /// <summary>
    /// Torob price comparison
    /// </summary>
    Torob = 1,
    
    /// <summary>
    /// Snapfood food delivery
    /// </summary>
    Snapfood = 2,
    
    /// <summary>
    /// Emalls marketplace
    /// </summary>
    Emalls = 3,
    
    /// <summary>
    /// Other marketplace
    /// </summary>
    Other = 99
}

/// <summary>
/// Represents the status of a proxy server
/// </summary>
public enum ProxyStatus
{
    /// <summary>
    /// Proxy is active and ready to use
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Proxy is blocked by target website
    /// </summary>
    Blocked = 1,
    
    /// <summary>
    /// Proxy is dead/unreachable
    /// </summary>
    Dead = 2
}
