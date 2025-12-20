namespace SmartPrice.Domain.Enums;

/// <summary>
/// Represents the frequency at which a job should run
/// </summary>
public enum JobFrequency
{
    /// <summary>
    /// One-time manual execution
    /// </summary>
    Manual = 0,
    
    /// <summary>
    /// Execute every hour
    /// </summary>
    Hourly = 1,
    
    /// <summary>
    /// Execute once per day
    /// </summary>
    Daily = 2,
    
    /// <summary>
    /// Execute once per week
    /// </summary>
    Weekly = 3,
    
    /// <summary>
    /// Custom schedule using cron expression
    /// </summary>
    Custom = 4
}

/// <summary>
/// Represents the priority level of a job
/// </summary>
public enum JobPriority
{
    /// <summary>
    /// Low priority job
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Normal priority job
    /// </summary>
    Normal = 1,
    
    /// <summary>
    /// High priority job
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Critical priority job
    /// </summary>
    Critical = 3
}
