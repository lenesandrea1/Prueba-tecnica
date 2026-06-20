using EventosVivos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventosVivos.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    private static readonly Venue[] ReferenceVenues =
    [
        new(1, "Auditorio Central", 200, "Bogotá"),
        new(2, "Sala Norte", 50, "Bogotá"),
        new(3, "Arena Sur", 500, "Medellín")
    ];

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Venues.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Venues.AddRange(ReferenceVenues);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} reference venues.", ReferenceVenues.Length);
    }
}
