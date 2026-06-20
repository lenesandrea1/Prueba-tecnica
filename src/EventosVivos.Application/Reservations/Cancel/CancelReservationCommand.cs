namespace EventosVivos.Application.Reservations.Cancel;

public sealed record CancelReservationCommand(Guid ReservationId);

public sealed record CancelReservationResult(
    Guid ReservationId,
    string Status,
    DateTime CancelledAtUtc);
