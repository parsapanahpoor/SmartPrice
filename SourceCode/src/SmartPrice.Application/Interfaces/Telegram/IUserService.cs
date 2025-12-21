using SmartPrice.Domain.Entities;

namespace SmartPrice.Application.Interfaces.Telegram;

/// <summary>
/// Service for managing Telegram users
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get an existing user or create a new one
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="username">Telegram username</param>
    /// <param name="firstName">User's first name</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user entity</returns>
    Task<TelegramUser> GetOrCreateUserAsync(long chatId, string? username, string? firstName, CancellationToken ct);

    /// <summary>
    /// Get a user by their chat ID
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user or null if not found</returns>
    Task<TelegramUser?> GetUserByChatIdAsync(long chatId, CancellationToken ct);

    /// <summary>
    /// Get a user by their ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>The user or null if not found</returns>
    Task<TelegramUser?> GetUserByIdAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Update the last interaction time for a user
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="ct">Cancellation token</param>
    Task UpdateUserInteractionAsync(long chatId, CancellationToken ct);

    /// <summary>
    /// Check if a user is an administrator
    /// </summary>
    /// <param name="chatId">Telegram chat ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>True if user is admin</returns>
    Task<bool> IsUserAdminAsync(long chatId, CancellationToken ct);

    /// <summary>
    /// Get total number of users
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Total user count</returns>
    Task<int> GetTotalUsersCountAsync(CancellationToken ct);

    /// <summary>
    /// Get total number of active users
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Active user count</returns>
    Task<int> GetActiveUsersCountAsync(CancellationToken ct);
}
