using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

internal sealed class ReservationRepository(AppDbContext dbContext) : IReservationRepository
{
    public Task AddAsync(Reservation entity, CancellationToken cancellationToken = default) =>
        dbContext.Reservations.AddAsync(entity, cancellationToken).AsTask();

    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Reservations
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Reservation>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.EventId == eventId)
            .ToListAsync(cancellationToken);

    public Task<bool> ConfirmationCodeExistsAsync(string code, CancellationToken cancellationToken = default) =>
        dbContext.Reservations
            .AsNoTracking()
            .AnyAsync(r => r.ConfirmationCode == code, cancellationToken);
}
