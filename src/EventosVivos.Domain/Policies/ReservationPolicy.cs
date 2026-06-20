using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Domain.Policies;

public static class ReservationPolicy
{
    public const int LateEventMaxQuantity = 5;
    public const int HighPriceMaxQuantity = 10;
    public const decimal HighPriceThreshold = 100m;

    public static readonly TimeSpan MinimumLeadTime = TimeSpan.FromHours(1);
    public static readonly TimeSpan LateEventWindow = TimeSpan.FromHours(24);
    public static readonly TimeSpan PenaltyWindow = TimeSpan.FromHours(48);

    public static void EnsureCanReserve(Event @event, int quantity, DateTime utcNow)
    {
        if (@event.Status != EventStatus.Activo)
        {
            throw new InvalidReservationException("Reservations are only allowed for active events.");
        }

        if (quantity < 1)
        {
            throw new InvalidReservationException("Quantity must be at least 1.");
        }

        var now = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
        var timeUntilStart = @event.StartAtUtc - now;

        if (timeUntilStart < MinimumLeadTime)
        {
            throw new InvalidReservationException(
                "Reservations are not allowed for events starting in less than 1 hour.");
        }

        if (timeUntilStart < LateEventWindow && quantity > LateEventMaxQuantity)
        {
            throw new InvalidReservationException(
                $"Events starting within 24 hours allow a maximum of {LateEventMaxQuantity} tickets per transaction.");
        }

        if (@event.TicketPrice > HighPriceThreshold && quantity > HighPriceMaxQuantity)
        {
            throw new InvalidReservationException(
                $"Events with ticket price above {HighPriceThreshold:C} allow a maximum of {HighPriceMaxQuantity} tickets per transaction.");
        }
    }

    public static int CountOccupiedSeats(IEnumerable<Reservation> reservations) =>
        reservations
            .Where(r => r.Status is ReservationStatus.Confirmada
                or ReservationStatus.PendientePago
                or ReservationStatus.Perdida)
            .Sum(r => r.Quantity);

    public static void EnsureCapacityAvailable(Event @event, int quantity, int occupiedSeats)
    {
        if (occupiedSeats + quantity > @event.MaxCapacity)
        {
            throw new InvalidReservationException(
                $"Only {@event.MaxCapacity - occupiedSeats} ticket(s) remain available for this event.");
        }
    }

    public static bool AppliesCancellationPenalty(DateTime eventStartAtUtc, DateTime utcNow) =>
        DateTime.SpecifyKind(eventStartAtUtc, DateTimeKind.Utc) - DateTime.SpecifyKind(utcNow, DateTimeKind.Utc)
        < PenaltyWindow;
}
