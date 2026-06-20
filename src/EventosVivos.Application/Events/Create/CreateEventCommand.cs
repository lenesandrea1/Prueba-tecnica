using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.Events.Create;

public sealed record CreateEventCommand(
    string Title,
    string Description,
    int VenueId,
    int MaxCapacity,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    decimal TicketPrice,
    EventType Type);

public sealed record CreateEventResult(
    Guid EventId,
    string Title,
    EventStatus Status);
