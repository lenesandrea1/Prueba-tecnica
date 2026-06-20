using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Policies;
using FluentAssertions;

namespace EventosVivos.Tests.Unit.Domain;

public sealed class ReservationPolicyTests
{
    private static readonly Venue Auditorium = new(1, "Auditorio Central", 200, "Bogotá");
    private static readonly DateTime UtcNow = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Event CreateEvent(decimal price = 50m, DateTime? start = null) =>
        Event.Create(
            Guid.NewGuid(),
            "Conferencia de prueba",
            "Descripción válida para pruebas de reservas.",
            Auditorium,
            100,
            start ?? UtcNow.AddDays(5),
            (start ?? UtcNow.AddDays(5)).AddHours(3),
            price,
            EventType.Conferencia,
            UtcNow,
            []);

    [Fact]
    public void EnsureCanReserve_Throws_WhenEventStartsInLessThanOneHour()
    {
        var @event = CreateEvent(start: UtcNow.AddMinutes(30));

        var action = () => ReservationPolicy.EnsureCanReserve(@event, 1, UtcNow);

        action.Should().Throw<InvalidReservationException>()
            .WithMessage("*less than 1 hour*");
    }

    [Fact]
    public void EnsureCanReserve_Throws_WhenQuantityExceedsLateEventLimit()
    {
        var @event = CreateEvent(start: UtcNow.AddHours(12));

        var action = () => ReservationPolicy.EnsureCanReserve(@event, 6, UtcNow);

        action.Should().Throw<InvalidReservationException>()
            .WithMessage("*24 hours*");
    }

    [Fact]
    public void EnsureCanReserve_Throws_WhenHighPriceEventExceedsQuantityLimit()
    {
        var @event = CreateEvent(price: 150m);

        var action = () => ReservationPolicy.EnsureCanReserve(@event, 11, UtcNow);

        action.Should().Throw<InvalidReservationException>()
            .WithMessage("*10 tickets*");
    }

    [Fact]
    public void CountOccupiedSeats_IncludesConfirmedPendingAndLost()
    {
        var @event = CreateEvent();
        var confirmed = Reservation.Create(Guid.NewGuid(), @event, 2, "Ana López", "ana@mail.com", UtcNow, 0);
        confirmed.ConfirmPayment("EV-000001", UtcNow);

        var pending = Reservation.Create(Guid.NewGuid(), @event, 3, "Luis Pérez", "luis@mail.com", UtcNow, 2);

        var nearEvent = CreateEvent(start: UtcNow.AddHours(30));
        var lost = Reservation.Create(Guid.NewGuid(), nearEvent, 4, "Eva Ruiz", "eva@mail.com", UtcNow, 0);
        lost.ConfirmPayment("EV-000002", UtcNow);
        lost.Cancel(nearEvent, UtcNow.AddHours(5));

        var occupied = ReservationPolicy.CountOccupiedSeats([confirmed, pending, lost]);

        occupied.Should().Be(9);
    }
}
