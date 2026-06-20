using EventosVivos.Application.Common;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Application.Reservations.ConfirmPayment;

public sealed class ConfirmReservationPaymentCommandHandler(
    IReservationRepository reservationRepository,
    IReservationCodeGenerator codeGenerator,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<ConfirmReservationPaymentResult> HandleAsync(
        ConfirmReservationPaymentCommand command,
        CancellationToken cancellationToken = default)
    {
        var reservation = await reservationRepository.GetByIdAsync(command.ReservationId, cancellationToken);
        if (reservation is null)
        {
            throw new NotFoundException($"Reservation {command.ReservationId} was not found.");
        }

        try
        {
            var code = await codeGenerator.GenerateUniqueAsync(cancellationToken);
            reservation.ConfirmPayment(code, timeProvider.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ConfirmReservationPaymentResult(
                reservation.Id,
                reservation.ConfirmationCode!,
                reservation.Status.ToString());
        }
        catch (InvalidReservationException ex)
        {
            throw new ConflictException(ex.Message);
        }
    }
}
