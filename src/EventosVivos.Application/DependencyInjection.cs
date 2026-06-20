using EventosVivos.Application.Events.Create;
using EventosVivos.Application.Events.List;
using Microsoft.Extensions.DependencyInjection;

namespace EventosVivos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateEventCommandHandler>();
        services.AddScoped<ListEventsQueryHandler>();
        return services;
    }
}
