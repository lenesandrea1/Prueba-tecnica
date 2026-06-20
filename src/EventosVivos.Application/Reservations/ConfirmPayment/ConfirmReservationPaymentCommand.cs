namespace EventosVivos.Application.Reservations.ConfirmPayment;

public sealed record ConfirmReservationPaymentCommand(Guid ReservationId);

public sealed record ConfirmReservationPaymentResult(
    Guid ReservationId,
    string ConfirmationCode,
    string Status);
