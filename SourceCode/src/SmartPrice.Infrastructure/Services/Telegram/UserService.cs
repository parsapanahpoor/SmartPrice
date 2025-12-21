using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces;
using SmartPrice.Application.Interfaces.Telegram;
using SmartPrice.Domain.Entities;

namespace SmartPrice.Infrastructure.Services.Telegram;

/// <summary>
/// Service for managing Telegram users
/// </summary>
public class UserService : IUserService
{
    private readonly IRepository<TelegramUser> _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IRepository<TelegramUser> userRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<TelegramUser> GetOrCreateUserAsync(long chatId, string? username, string? firstName, CancellationToken ct)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.ChatId == chatId,
            ct);

        if (user != null)
        {
            // Update user info if changed
            if (user.Username != username || user.FirstName != firstName)
            {
                user.Username = username;
                user.FirstName = firstName;
                user.LastInteractionAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user, ct);
            }
            return user;
        }

        // Create new user
        user = new TelegramUser
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            Username = username,
            FirstName = firstName,
            IsActive = true,
            NotificationsEnabled = true,
            LanguageCode = "fa",
            LastInteractionAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, ct);

        _logger.LogInformation("New Telegram user created: ChatId={ChatId}, Username={Username}",
            chatId, username);

        return user;
    }

    public async Task<TelegramUser?> GetUserByChatIdAsync(long chatId, CancellationToken ct)
    {
        return await _userRepository.FirstOrDefaultAsync(
            u => u.ChatId == chatId,
            ct);
    }

    public async Task<TelegramUser?> GetUserByIdAsync(Guid userId, CancellationToken ct)
    {
        return await _userRepository.GetByIdAsync(userId, ct);
    }

    public async Task UpdateUserInteractionAsync(long chatId, CancellationToken ct)
    {
        var user = await GetUserByChatIdAsync(chatId, ct);
        if (user == null) return;

        user.LastInteractionAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, ct);
    }

    public async Task<bool> IsUserAdminAsync(long chatId, CancellationToken ct)
    {
        var user = await GetUserByChatIdAsync(chatId, ct);
        return user?.IsAdmin ?? false;
    }

    public async Task<int> GetTotalUsersCountAsync(CancellationToken ct)
    {
        return await _userRepository.CountAsync(u => true, ct);
    }

    public async Task<int> GetActiveUsersCountAsync(CancellationToken ct)
    {
        return await _userRepository.CountAsync(u => u.IsActive, ct);
    }
}
