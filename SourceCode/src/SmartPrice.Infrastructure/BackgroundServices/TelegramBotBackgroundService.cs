using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartPrice.Application.Interfaces.Telegram;

namespace SmartPrice.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that manages the Telegram bot lifecycle
/// </summary>
public class TelegramBotBackgroundService : BackgroundService
{
    private readonly ITelegramBotService _botService;
    private readonly ILogger<TelegramBotBackgroundService> _logger;

    public TelegramBotBackgroundService(
        ITelegramBotService botService,
        ILogger<TelegramBotBackgroundService> logger)
    {
        _botService = botService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram Bot Background Service is starting");

        // Wait a bit for application to fully initialize
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        try
        {
            await _botService.StartAsync(stoppingToken);

            _logger.LogInformation("Telegram Bot is now running");

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Telegram Bot Background Service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Telegram Bot Background Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telegram Bot Background Service is stopping");

        await _botService.StopAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
