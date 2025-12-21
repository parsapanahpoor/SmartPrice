namespace SmartPrice.Domain.Enums;

/// <summary>
/// Types of notifications that can be sent to users
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Price decreased notification
    /// </summary>
    PriceDropped = 0,
    
    /// <summary>
    /// Price increased notification
    /// </summary>
    PriceIncreased = 1,
    
    /// <summary>
    /// Target price reached notification
    /// </summary>
    TargetPriceReached = 2,
    
    /// <summary>
    /// Product availability changed
    /// </summary>
    AvailabilityChanged = 3,
    
    /// <summary>
    /// Welcome message for new users
    /// </summary>
    Welcome = 4,
    
    /// <summary>
    /// Daily report notification
    /// </summary>
    DailyReport = 5,
    
    /// <summary>
    /// System alert notification
    /// </summary>
    SystemAlert = 6
}

/// <summary>
/// Bot commands available to users
/// </summary>
public enum BotCommand
{
    /// <summary>
    /// Start command - initialize bot
    /// </summary>
    Start = 0,
    
    /// <summary>
    /// Help command - show help message
    /// </summary>
    Help = 1,
    
    /// <summary>
    /// Track command - track a product
    /// </summary>
    Track = 2,
    
    /// <summary>
    /// Untrack command - stop tracking a product
    /// </summary>
    Untrack = 3,
    
    /// <summary>
    /// MyProducts command - show user's tracked products
    /// </summary>
    MyProducts = 4,
    
    /// <summary>
    /// Settings command - user settings
    /// </summary>
    Settings = 5,
    
    /// <summary>
    /// Stats command - system statistics (admin only)
    /// </summary>
    Stats = 6,
    
    /// <summary>
    /// Cancel command - cancel current operation
    /// </summary>
    Cancel = 7
}
