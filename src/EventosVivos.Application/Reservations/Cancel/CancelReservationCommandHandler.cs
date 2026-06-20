using EventosVivos.Application.Common;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Application.Reservations.Cancel;

public sealed class CancelReservationCommandHandler(
    IEventRepository eventRepository,
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<CancelReservationResult> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var reservation = await reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new NotFoundException($"Reservation {command.ReservationId} was not found.");
        }

        var @event = await eventRepository.GetByIdAsync(reservation.EventId, cancellationToken);
        if (@event is null)
        {
            throw new NotFoundException($"Event {reservation.EventId} was not found.");
        }

        try
        {
            reservation.Cancel(@event, timeProvider.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new CancelReservationResult(
                reservation.Id,
                reservation.Status.ToString(),
                reservation.CancelledAtUtc!.Value);
        }
        catch (InvalidReservationException ex)
        {
            throw new ConflictException(ex.Message);
        }
    }
}
