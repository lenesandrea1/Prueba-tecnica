using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Services;

internal sealed class ReservationCodeGenerator(AppDbContext dbContext) : IReservationCodeGenerator
{
    private const int MaxAttempts = 20;

    public async Task<string> GenerateUniqueAsync(CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var code = $"EV-{Random.Shared.Next(0, 1_000_000):D6}";
            var exists = await dbContext.Reservations
                .AsNoTracking()
                .AnyAsync(r => r.ConfirmationCode == code, cancellationToken);

            if (!exists)
            {
                return code;
            }
        }

        throw new InvalidOperationException("Unable to generate a unique reservation code.");
    }
}
