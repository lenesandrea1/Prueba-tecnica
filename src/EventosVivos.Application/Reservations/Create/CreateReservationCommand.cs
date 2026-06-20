namespace EventosVivos.Application.Reservations.Create;

public sealed record CreateReservationCommand(
    Guid EventId,
    int Quantity,
    string BuyerName,
    string BuyerEmail);

public sealed record CreateReservationResult(
    Guid ReservationId,
    Guid EventId,
    int Quantity,
    string Status);
