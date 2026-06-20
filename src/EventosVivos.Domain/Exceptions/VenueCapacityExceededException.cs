namespace EventosVivos.Domain.Exceptions;

public sealed class VenueCapacityExceededException(int venueCapacity, int requestedCapacity)
    : DomainException($"Event capacity ({requestedCapacity}) exceeds venue capacity ({venueCapacity}).")
{
    public int VenueCapacity { get; } = venueCapacity;
    public int RequestedCapacity { get; } = requestedCapacity;
}
