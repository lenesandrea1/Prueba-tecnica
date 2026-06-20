namespace EventosVivos.Domain.Exceptions;

public sealed class VenueScheduleConflictException(int venueId, DateTime startAtUtc, DateTime endAtUtc)
    : DomainException($"Venue {venueId} already has an active event overlapping [{startAtUtc:O}, {endAtUtc:O}).")
{
    public int VenueId { get; } = venueId;
    public DateTime StartAtUtc { get; } = startAtUtc;
    public DateTime EndAtUtc { get; } = endAtUtc;
}
