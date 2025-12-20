namespace SmartPrice.Application.DTOs;

/// <summary>
/// Data transfer object for scraping operation results
/// </summary>
/// <param name="TotalProducts">Total number of products processed</param>
/// <param name="NewProducts">Number of new products added</param>
/// <param name="UpdatedProducts">Number of existing products updated</param>
/// <param name="Duration">Time taken for the operation</param>
/// <param name="IsSuccess">Whether the operation was successful</param>
/// <param name="ErrorMessage">Error message if operation failed</param>
public record ScrapingResultDto(
    int TotalProducts,
    int NewProducts,
    int UpdatedProducts,
    TimeSpan Duration,
    bool IsSuccess,
    string? ErrorMessage
);
