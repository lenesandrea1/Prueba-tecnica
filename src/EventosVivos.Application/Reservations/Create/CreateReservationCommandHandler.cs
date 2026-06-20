using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Application.Reservations.Create;

public sealed class CreateReservationCommandHandler(
    IEventRepository eventRepository,
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<CreateReservationResult> HandleAsync(
        CreateReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var @event = await eventRepository.GetByIdAsync(command.EventId, cancellationToken);
        if (@event is null)
        {
            throw new NotFoundException($"Event {command.EventId} was not found.");
        }

        var existingReservations = await reservationRepository.GetByEventIdAsync(command.EventId, cancellationToken);
        var occupiedSeats = Domain.Policies.ReservationPolicy.CountOccupiedSeats(existingReservations);

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            @event,
            command.Quantity,
            command.BuyerName,
            command.BuyerEmail,
            timeProvider.GetUtcNow().UtcDateTime,
            occupiedSeats);

        await reservationRepository.AddAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateReservationResult(
            reservation.Id,
            reservation.EventId,
            reservation.Quantity,
            reservation.Status.ToString());
    }
}
