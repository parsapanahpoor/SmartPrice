using SmartPrice.Application.DTOs.Telegram;

namespace SmartPrice.Application.Interfaces.Telegram;

/// <summary>
/// Service for handling Telegram bot commands
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Handle incoming command from Telegram
    /// </summary>
    /// <param name="message">Telegram message DTO</param>
    /// <param name="ct">Cancellation token</param>
    Task HandleCommandAsync(TelegramMessageDto message, CancellationToken ct);
}
