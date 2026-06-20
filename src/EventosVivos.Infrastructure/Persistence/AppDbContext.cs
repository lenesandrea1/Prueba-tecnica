using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
