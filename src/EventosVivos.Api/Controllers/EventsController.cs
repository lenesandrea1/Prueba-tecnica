using EventosVivos.Application.Events.Create;
using EventosVivos.Application.Events.List;
using EventosVivos.Application.Events.Reports;
using EventosVivos.Application.Reservations.Create;
using EventosVivos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(
    CreateEventCommandHandler createEventHandler,
    ListEventsQueryHandler listEventsHandler,
    CreateReservationCommandHandler createReservationHandler,
    GetOccupancyReportQueryHandler occupancyReportHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Contracts.Events.CreateEventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] Contracts.Events.CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createEventHandler.HandleAsync(
            Contracts.Events.CreateEventRequestMapping.ToCommand(request),
            cancellationToken);

        var response = new Contracts.Events.CreateEventResponse(
            result.EventId,
            result.Title,
            result.Status);

        return CreatedAtAction(nameof(List), new { id = result.EventId }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] EventType? type,
        [FromQuery] int? venueId,
        [FromQuery] EventStatus? status,
        [FromQuery] DateTime? startFromUtc,
        [FromQuery] DateTime? startToUtc,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var query = new ListEventsQuery(
            type,
            venueId,
            status,
            startFromUtc.HasValue ? DateTime.SpecifyKind(startFromUtc.Value, DateTimeKind.Utc) : null,
            startToUtc.HasValue ? DateTime.SpecifyKind(startToUtc.Value, DateTimeKind.Utc) : null,
            search);

        var result = await listEventsHandler.HandleAsync(query, cancellationToken);
        return Ok(result.Items);
    }

    [HttpPost("{eventId:guid}/reservations")]
    [ProducesResponseType(typeof(Contracts.Reservations.CreateReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateReservation(
        Guid eventId,
        [FromBody] Contracts.Reservations.CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createReservationHandler.HandleAsync(
            Contracts.Reservations.CreateReservationRequestMapping.ToCommand(eventId, request),
            cancellationToken);

        var response = new Contracts.Reservations.CreateReservationResponse(
            result.ReservationId,
            result.EventId,
            result.Quantity,
            result.Status);

        return CreatedAtAction(
            nameof(ReservationsController.ConfirmPayment),
            "Reservations",
            new { reservationId = result.ReservationId },
            response);
    }

    [HttpGet("{eventId:guid}/occupancy-report")]
    [ProducesResponseType(typeof(OccupancyReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOccupancyReport(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var result = await occupancyReportHandler.HandleAsync(
            new GetOccupancyReportQuery(eventId),
            cancellationToken);

        return Ok(new OccupancyReportResponse(
            result.EventId,
            result.EventTitle,
            result.MaxCapacity,
            result.SoldTickets,
            result.AvailableTickets,
            result.OccupancyPercentage,
            result.TotalRevenue,
            result.EventStatus));
    }
}

public sealed record OccupancyReportResponse(
    Guid EventId,
    string EventTitle,
    int MaxCapacity,
    int SoldTickets,
    int AvailableTickets,
    decimal OccupancyPercentage,
    decimal TotalRevenue,
    EventStatus EventStatus);
