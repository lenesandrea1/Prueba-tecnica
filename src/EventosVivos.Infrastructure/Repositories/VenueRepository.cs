using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

internal sealed class VenueRepository(AppDbContext dbContext) : IVenueRepository
{
    public Task<Venue?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Venue>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Venues.AsNoTracking().OrderBy(v => v.Id).ToListAsync(cancellationToken);
}
