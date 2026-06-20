using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;

namespace EventosVivos.Application.Venues.List;

public sealed record VenueListItemDto(int Id, string Name, int Capacity, string City);

public sealed class ListVenuesQueryHandler(IVenueRepository venueRepository)
{
    public async Task<IReadOnlyList<VenueListItemDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var venues = await venueRepository.GetAllAsync(cancellationToken);
        return venues
            .Select(v => new VenueListItemDto(v.Id, v.Name, v.Capacity, v.City))
            .ToList();
    }
}
