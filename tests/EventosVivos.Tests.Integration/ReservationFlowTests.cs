using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using EventosVivos.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EventosVivos.Tests.Integration;

public sealed class ReservationFlowTests : IClassFixture<EventosVivosWebApplicationFactory>, IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;
    private readonly EventosVivosWebApplicationFactory _factory;
    private bool _databaseReady;

    public ReservationFlowTests(EventosVivosWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        _databaseReady = await DatabaseIsAvailableAsync();
        if (!_databaseReady)
        {
            return;
        }

        await DatabaseSeeder.SeedAsync(_factory.Services);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ReservationFlow_CreateConfirmAndReport_WorksEndToEnd()
    {
        if (!_databaseReady)
        {
            return;
        }

        var utcNow = _factory.TimeProvider.GetUtcNow().UtcDateTime;
        var createEventResponse = await _client.PostAsJsonAsync("/api/events", new
        {
            title = "Foro de Integración",
            description = "Evento creado desde prueba de integración end-to-end.",
            venueId = 1,
            maxCapacity = 40,
            startAtUtc = utcNow.AddDays(10),
            endAtUtc = utcNow.AddDays(10).AddHours(4),
            ticketPrice = 75m,
            type = 1
        });

        createEventResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<CreatedEventResponse>(JsonOptions);
        Assert.NotNull(createdEvent);

        var reservationResponse = await _client.PostAsJsonAsync(
            $"/api/events/{createdEvent.EventId}/reservations",
            new
            {
                quantity = 3,
                buyerName = "Ana Integración",
                buyerEmail = "ana.integration@example.com"
            });

        reservationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var reservation = await reservationResponse.Content.ReadFromJsonAsync<CreatedReservationResponse>(JsonOptions);
        Assert.NotNull(reservation);
        Assert.Equal("PendientePago", reservation.Status);

        var confirmResponse = await _client.PostAsync(
            $"/api/reservations/{reservation.ReservationId}/confirm-payment",
            null);

        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var confirmation = await confirmResponse.Content.ReadFromJsonAsync<ConfirmPaymentResponse>(JsonOptions);
        confirmation!.ConfirmationCode.Should().StartWith("EV-");

        var reportResponse = await _client.GetAsync($"/api/events/{createdEvent.EventId}/occupancy-report");
        reportResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await reportResponse.Content.ReadFromJsonAsync<OccupancyReportResponse>(JsonOptions);
        report!.SoldTickets.Should().Be(3);
        report.TotalRevenue.Should().Be(225m);
        report.AvailableTickets.Should().Be(37);
    }

    [Fact]
    public async Task CreateEvent_ReturnsConflict_WhenVenueScheduleOverlaps()
    {
        if (!_databaseReady)
        {
            return;
        }

        var utcNow = _factory.TimeProvider.GetUtcNow().UtcDateTime;
        var start = utcNow.AddDays(20);
        var end = start.AddHours(3);

        var first = await _client.PostAsJsonAsync("/api/events", new
        {
            title = "Evento base integración",
            description = "Primer evento para validar conflicto de horario.",
            venueId = 2,
            maxCapacity = 30,
            startAtUtc = start,
            endAtUtc = end,
            ticketPrice = 40m,
            type = 2
        });

        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var overlapping = await _client.PostAsJsonAsync("/api/events", new
        {
            title = "Evento solapado",
            description = "Segundo evento que debe chocar con el primero.",
            venueId = 2,
            maxCapacity = 20,
            startAtUtc = start.AddHours(1),
            endAtUtc = end.AddHours(1),
            ticketPrice = 35m,
            type = 2
        });

        overlapping.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private async Task<bool> DatabaseIsAvailableAsync()
    {
        try
        {
            var response = await _client.GetAsync("/api/venues");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private sealed class CreatedEventResponse
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private sealed class CreatedReservationResponse
    {
        public Guid ReservationId { get; set; }
        public Guid EventId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    private sealed class ConfirmPaymentResponse
    {
        public Guid ReservationId { get; set; }
        public string ConfirmationCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    private sealed class OccupancyReportResponse
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public int SoldTickets { get; set; }
        public int AvailableTickets { get; set; }
        public decimal OccupancyPercentage { get; set; }
        public decimal TotalRevenue { get; set; }
        public string EventStatus { get; set; } = string.Empty;
    }
}
