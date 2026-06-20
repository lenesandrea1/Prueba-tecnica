using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Guards;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

internal sealed class EventRepository(AppDbContext dbContext) : IEventRepository
{
    public Task AddAsync(Event entity, CancellationToken cancellationToken = default) =>
        dbContext.Events.AddAsync(entity, cancellationToken).AsTask();

    public async Task<IReadOnlyList<Event>> GetActiveOverlappingAsync(
        int venueId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        CancellationToken cancellationToken = default)
    {
        var start = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);

        return await dbContext.Events
            .AsNoTracking()
            .Where(e => e.VenueId == venueId && e.Status == EventStatus.Activo)
            .Where(e => e.StartAtUtc < end && start < e.EndAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> SearchAsync(
        EventSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Events
            .AsNoTracking()
            .Include(e => e.Venue)
            .AsQueryable();

        if (criteria.Type.HasValue)
        {
            query = query.Where(e => e.Type == criteria.Type.Value);
        }

        if (criteria.VenueId.HasValue)
        {
            query = query.Where(e => e.VenueId == criteria.VenueId.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(e => e.Status == criteria.Status.Value);
        }

        if (criteria.StartFromUtc.HasValue)
        {
            var from = DateTime.SpecifyKind(criteria.StartFromUtc.Value, DateTimeKind.Utc);
            query = query.Where(e => e.StartAtUtc >= from);
        }

        if (criteria.StartToUtc.HasValue)
        {
            var to = DateTime.SpecifyKind(criteria.StartToUtc.Value, DateTimeKind.Utc);
            query = query.Where(e => e.StartAtUtc <= to);
        }

        if (!string.IsNullOrWhiteSpace(criteria.TitleSearch))
        {
            var term = criteria.TitleSearch.Trim().ToLowerInvariant();
            query = query.Where(e => e.Title.ToLower().Contains(term));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Events
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
}
