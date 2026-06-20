using EventosVivos.Application.Events.Create;
using EventosVivos.Application.Events.List;
using EventosVivos.Application.Events.Reports;
using EventosVivos.Application.Reservations.Cancel;
using EventosVivos.Application.Reservations.ConfirmPayment;
using EventosVivos.Application.Reservations.Create;
using EventosVivos.Application.Venues.List;
using Microsoft.Extensions.DependencyInjection;
namespace EventosVivos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateEventCommandHandler>();
        services.AddScoped<ListEventsQueryHandler>();
        services.AddScoped<CreateReservationCommandHandler>();
        services.AddScoped<ConfirmReservationPaymentCommandHandler>();
        services.AddScoped<CancelReservationCommandHandler>();
        services.AddScoped<GetOccupancyReportQueryHandler>();
        services.AddScoped<ListVenuesQueryHandler>();
        return services;
    }
}
