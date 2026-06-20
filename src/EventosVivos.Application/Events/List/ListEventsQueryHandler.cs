using EventosVivos.Application.Common;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.Events.List;

public sealed class ListEventsQueryHandler(
    IEventRepository eventRepository,
    TimeProvider timeProvider)
{
    public async Task<ListEventsResult> HandleAsync(
        ListEventsQuery query,
        CancellationToken cancellationToken = default)
    {
        var criteria = new EventSearchCriteria(
            query.Type,
            query.VenueId,
            query.Status,
            query.StartFromUtc,
            query.StartToUtc,
            query.TitleSearch);

        var events = await eventRepository.SearchAsync(criteria, cancellationToken);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var items = events
            .Select(entity => new EventListItemDto(
                    entity.Id,
                    entity.Title,
                    entity.Description,
                    entity.VenueId,
                    entity.Venue?.Name ?? string.Empty,
                    entity.Venue?.City ?? string.Empty,
                    entity.MaxCapacity,
                    entity.StartAtUtc,
                    entity.EndAtUtc,
                    entity.TicketPrice,
                    entity.Type,
                    ResolveStatus(entity, utcNow)))
            .OrderBy(item => item.StartAtUtc)
            .ToList();

        return new ListEventsResult(items);
    }

    private static EventStatus ResolveStatus(Domain.Entities.Event entity, DateTime utcNow)
    {
        if (entity.Status == EventStatus.Activo && utcNow >= entity.EndAtUtc)
        {
            return EventStatus.Completado;
        }

        return entity.Status;
    }
}
