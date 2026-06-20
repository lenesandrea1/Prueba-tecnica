using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.Common;

public interface IEventRepository
{
    Task AddAsync(Event entity, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetActiveOverlappingAsync(
        int venueId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> SearchAsync(
        EventSearchCriteria criteria,
        CancellationToken cancellationToken = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venue>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record EventSearchCriteria(
    EventType? Type,
    int? VenueId,
    EventStatus? Status,
    DateTime? StartFromUtc,
    DateTime? StartToUtc,
    string? TitleSearch);
