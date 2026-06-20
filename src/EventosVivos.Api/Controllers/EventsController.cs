using EventosVivos.Application.Events.Create;
using EventosVivos.Application.Events.List;
using EventosVivos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(
    CreateEventCommandHandler createEventHandler,
    ListEventsQueryHandler listEventsHandler) : ControllerBase
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
}
