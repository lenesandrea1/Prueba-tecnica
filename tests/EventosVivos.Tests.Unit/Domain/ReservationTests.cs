using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using FluentAssertions;

namespace EventosVivos.Tests.Unit.Domain;

public sealed class ReservationTests
{
    private static readonly Venue Auditorium = new(1, "Auditorio Central", 200, "Bogotá");
    private static readonly DateTime UtcNow = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Event CreateEvent(DateTime? start = null) =>
        Event.Create(
            Guid.NewGuid(),
            "Conferencia de prueba",
            "Descripción válida para pruebas de reservas.",
            Auditorium,
            100,
            start ?? UtcNow.AddDays(5),
            (start ?? UtcNow.AddDays(5)).AddHours(3),
            80m,
            EventType.Conferencia,
            UtcNow,
            []);

    [Fact]
    public void Create_InitializesPendingPaymentReservation()
    {
        var @event = CreateEvent();

        var reservation = Reservation.Create(
            Guid.NewGuid(),
            @event,
            2,
            "María Gómez",
            "maria@example.com",
            UtcNow,
            0);

        reservation.Status.Should().Be(ReservationStatus.PendientePago);
        reservation.ConfirmationCode.Should().BeNull();
    }

    [Fact]
    public void ConfirmPayment_AssignsCode_WhenPending()
    {
        var @event = CreateEvent();
        var reservation = Reservation.Create(
            Guid.NewGuid(), @event, 1, "Pedro Ruiz", "pedro@example.com", UtcNow, 0);

        reservation.ConfirmPayment("EV-123456", UtcNow);

        reservation.Status.Should().Be(ReservationStatus.Confirmada);
        reservation.ConfirmationCode.Should().Be("EV-123456");
    }

    [Fact]
    public void ConfirmPayment_Throws_WhenAlreadyConfirmed()
    {
        var @event = CreateEvent();
        var reservation = Reservation.Create(
            Guid.NewGuid(), @event, 1, "Pedro Ruiz", "pedro@example.com", UtcNow, 0);
        reservation.ConfirmPayment("EV-123456", UtcNow);

        var action = () => reservation.ConfirmPayment("EV-999999", UtcNow);

        action.Should().Throw<InvalidReservationException>()
            .WithMessage("*already confirmed*");
    }

    [Fact]
    public void Cancel_MarksPerdida_WhenConfirmedNearEventStart()
    {
        var start = UtcNow.AddHours(24);
        var @event = CreateEvent(start);
        var reservation = Reservation.Create(
            Guid.NewGuid(), @event, 4, "Laura Díaz", "laura@example.com", UtcNow, 0);
        reservation.ConfirmPayment("EV-654321", UtcNow);

        reservation.Cancel(@event, UtcNow.AddHours(2));

        reservation.Status.Should().Be(ReservationStatus.Perdida);
        reservation.CancelledAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_ReleasesSeats_WhenConfirmedWithEnoughLeadTime()
    {
        var @event = CreateEvent();
        var reservation = Reservation.Create(
            Guid.NewGuid(), @event, 4, "Laura Díaz", "laura@example.com", UtcNow, 0);
        reservation.ConfirmPayment("EV-654321", UtcNow);

        reservation.Cancel(@event, UtcNow.AddDays(1));

        reservation.Status.Should().Be(ReservationStatus.Cancelada);
    }
}
