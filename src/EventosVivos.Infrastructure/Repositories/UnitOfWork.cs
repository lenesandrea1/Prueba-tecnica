using EventosVivos.Application.Common;
using EventosVivos.Infrastructure.Persistence;

namespace EventosVivos.Infrastructure.Repositories;

internal sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
