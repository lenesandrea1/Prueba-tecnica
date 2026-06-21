using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Guards;

namespace EventosVivos.Domain.Entities;

public sealed class Event
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int VenueId { get; private set; }
    public Venue? Venue { get; private set; }
    public int MaxCapacity { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }
    public decimal TicketPrice { get; private set; }
    public EventType Type { get; private set; }
    public EventStatus Status { get; private set; } = EventStatus.Activo;

    private Event()
    {
    }

    public static Event Create(
        Guid id,
        string title,
        string description,
        Venue venue,
        int maxCapacity,
        DateTime startAtUtc,
        DateTime endAtUtc,
        decimal ticketPrice,
        EventType type,
        DateTime utcNow,
        IReadOnlyCollection<Event> overlappingActiveEvents)
    {
        ValidateContent(title, description, maxCapacity, startAtUtc, endAtUtc, ticketPrice, utcNow);
        EventScheduleGuard.EnsureWeekendNightRestriction(startAtUtc);
        EventScheduleGuard.EnsureNoVenueOverlap(venue.Id, startAtUtc, endAtUtc, overlappingActiveEvents);

        if (maxCapacity > venue.Capacity)
        {
            throw new VenueCapacityExceededException(venue.Capacity, maxCapacity);
        }

        return new Event
        {
            Id = id,
            Title = title.Trim(),
            Description = description.Trim(),
            VenueId = venue.Id,
            MaxCapacity = maxCapacity,
            StartAtUtc = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc),
            EndAtUtc = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc),
            TicketPrice = ticketPrice,
            Type = type,
            Status = EventStatus.Activo
        };
    }

    public void MarkCompleted(DateTime utcNow)
    {
        if (Status != EventStatus.Activo)
        {
            return;
        }

        if (utcNow >= EndAtUtc)
        {
            Status = EventStatus.Completado;
        }
    }

    private static void ValidateContent(
        string title,
        string description,
        int maxCapacity,
        DateTime startAtUtc,
        DateTime endAtUtc,
        decimal ticketPrice,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length is < 5 or > 100)
        {
            throw new InvalidEventScheduleException("Title must be between 5 and 100 characters.");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length is < 10 or > 500)
        {
            throw new InvalidEventScheduleException("Description must be between 10 and 500 characters.");
        }

        if (maxCapacity <= 0)
        {
            throw new InvalidEventScheduleException("Max capacity must be a positive integer.");
        }

        if (ticketPrice <= 0)
        {
            throw new InvalidEventScheduleException("Ticket price must be greater than zero.");
        }

        var start = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);
        var now = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

        if (start <= now)
        {
            throw new InvalidEventScheduleException("Event start must be in the future.");
        }

        if (end <= start)
        {
            throw new InvalidEventScheduleException("Event end must be after the start time.");
        }
    }
}
