namespace SmartPrice.Domain.Enums;

/// <summary>
/// Represents the status of a scraping job
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// Job is waiting to be executed
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Job is currently running
    /// </summary>
    Running = 1,
    
    /// <summary>
    /// Job completed successfully
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Job failed due to an error
    /// </summary>
    Failed = 3
}
