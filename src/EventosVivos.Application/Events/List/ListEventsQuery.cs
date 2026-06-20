using EventosVivos.Application.Common;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.Events.List;

public sealed record ListEventsQuery(
    EventType? Type,
    int? VenueId,
    EventStatus? Status,
    DateTime? StartFromUtc,
    DateTime? StartToUtc,
    string? TitleSearch);

public sealed record EventListItemDto(
    Guid Id,
    string Title,
    string Description,
    int VenueId,
    string VenueName,
    string VenueCity,
    int MaxCapacity,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    decimal TicketPrice,
    EventType Type,
    EventStatus Status);

public sealed record ListEventsResult(IReadOnlyList<EventListItemDto> Items);
