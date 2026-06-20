using EventosVivos.Application.Events.Create;
using EventosVivos.Domain.Enums;
using FluentValidation;

namespace EventosVivos.Api.Contracts.Events;

public sealed record CreateEventRequest(
    string Title,
    string Description,
    int VenueId,
    int MaxCapacity,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    decimal TicketPrice,
    EventType Type);

public sealed record CreateEventResponse(
    Guid EventId,
    string Title,
    EventStatus Status);

public sealed class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(5, 100);
        RuleFor(x => x.Description).NotEmpty().Length(10, 500);
        RuleFor(x => x.VenueId).GreaterThan(0);
        RuleFor(x => x.MaxCapacity).GreaterThan(0);
        RuleFor(x => x.TicketPrice).GreaterThan(0);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.EndAtUtc).GreaterThan(x => x.StartAtUtc)
            .WithMessage("End time must be after start time.");
    }
}

public static class CreateEventRequestMapping
{
    public static CreateEventCommand ToCommand(CreateEventRequest request) =>
        new(
            request.Title,
            request.Description,
            request.VenueId,
            request.MaxCapacity,
            DateTime.SpecifyKind(request.StartAtUtc, DateTimeKind.Utc),
            DateTime.SpecifyKind(request.EndAtUtc, DateTimeKind.Utc),
            request.TicketPrice,
            request.Type);
}
