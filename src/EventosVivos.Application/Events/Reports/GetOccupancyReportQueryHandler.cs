using EventosVivos.Application.Common;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Policies;

namespace EventosVivos.Application.Events.Reports;

public sealed class GetOccupancyReportQueryHandler(
    IEventRepository eventRepository,
    IReservationRepository reservationRepository,
    TimeProvider timeProvider)
{
    public async Task<OccupancyReportResult> HandleAsync(
        GetOccupancyReportQuery query,
        CancellationToken cancellationToken = default)
    {
        var @event = await eventRepository.GetByIdAsync(query.EventId, cancellationToken);
        if (@event is null)
        {
            throw new NotFoundException($"Event {query.EventId} was not found.");
        }

        var reservations = await reservationRepository.GetByEventIdAsync(query.EventId, cancellationToken);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var soldTickets = reservations
            .Where(r => r.Status == ReservationStatus.Confirmada)
            .Sum(r => r.Quantity);

        var occupiedSeats = ReservationPolicy.CountOccupiedSeats(reservations);
        var availableTickets = Math.Max(0, @event.MaxCapacity - occupiedSeats);
        var occupancyPercentage = @event.MaxCapacity == 0
            ? 0m
            : Math.Round((decimal)occupiedSeats / @event.MaxCapacity * 100m, 2);

        var totalRevenue = soldTickets * @event.TicketPrice;
        var status = ResolveStatus(@event, utcNow);

        return new OccupancyReportResult(
            @event.Id,
            @event.Title,
            @event.MaxCapacity,
            soldTickets,
            availableTickets,
            occupancyPercentage,
            totalRevenue,
            status);
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
