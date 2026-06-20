using EventosVivos.Application.Reservations.Create;
using FluentValidation;

namespace EventosVivos.Api.Contracts.Reservations;

public sealed record CreateReservationRequest(
    int Quantity,
    string BuyerName,
    string BuyerEmail);

public sealed record CreateReservationResponse(
    Guid ReservationId,
    Guid EventId,
    int Quantity,
    string Status);

public sealed class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
        RuleFor(x => x.BuyerName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.BuyerEmail).NotEmpty().EmailAddress();
    }
}

public static class CreateReservationRequestMapping
{
    public static CreateReservationCommand ToCommand(Guid eventId, CreateReservationRequest request) =>
        new(eventId, request.Quantity, request.BuyerName, request.BuyerEmail);
}
