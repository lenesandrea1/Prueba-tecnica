using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;

namespace EventosVivos.Tests.Integration;

public sealed class EventosVivosWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeTimeProvider TimeProvider { get; } = new(new DateTimeOffset(2026, 6, 1, 12, 0, 0, TimeSpan.Zero));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=eventosvivos;Username=eventos;Password=local";

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            });
        });

        builder.ConfigureServices(services =>
        {
            var hostedServices = services
                .Where(descriptor => descriptor.ServiceType == typeof(IHostedService)
                    && descriptor.ImplementationType?.Name == "EventCompletionHostedService")
                .ToList();

            foreach (var descriptor in hostedServices)
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(TimeProvider);
        });
    }
}
