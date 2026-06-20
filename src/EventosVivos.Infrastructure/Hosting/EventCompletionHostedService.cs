using EventosVivos.Domain.Enums;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventosVivos.Infrastructure.Hosting;

internal sealed class EventCompletionHostedService(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<EventCompletionHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CompleteExpiredEventsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Failed to complete expired events.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CompleteExpiredEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var expiredEvents = await dbContext.Events
            .Where(e => e.Status == EventStatus.Activo && e.EndAtUtc <= utcNow)
            .ToListAsync(cancellationToken);

        if (expiredEvents.Count == 0)
        {
            return;
        }

        foreach (var @event in expiredEvents)
        {
            @event.MarkCompleted(utcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Marked {Count} event(s) as completed.", expiredEvents.Count);
    }
}
