using EventosVivos.Application.Venues.List;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/venues")]
public sealed class VenuesController(ListVenuesQueryHandler listVenuesHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VenueListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var venues = await listVenuesHandler.HandleAsync(cancellationToken);
        return Ok(venues);
    }
}
