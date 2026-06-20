using EventosVivos.Application.Common;
using EventosVivos.Application.Reservations.ConfirmPayment;
using EventosVivos.Application.Reservations.Create;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace EventosVivos.Tests.Unit.Application;

public sealed class ReservationHandlersTests
{
    private static readonly Venue Auditorium = new(1, "Auditorio Central", 200, "Bogotá");
    private static readonly DateTime UtcNow = new(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Guid EventId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static Event SampleEvent() =>
        Event.Create(
            EventId,
            "Conferencia de prueba",
            "Descripción válida para pruebas de reservas.",
            Auditorium,
            50,
            UtcNow.AddDays(5),
            UtcNow.AddDays(5).AddHours(3),
            120m,
            EventType.Conferencia,
            UtcNow,
            []);

    [Fact]
    public async Task CreateReservation_ThrowsNotFound_WhenEventMissing()
    {
        var eventRepository = Substitute.For<IEventRepository>();
        eventRepository.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var handler = new CreateReservationCommandHandler(
            eventRepository,
            Substitute.For<IReservationRepository>(),
            Substitute.For<IUnitOfWork>(),
            new FakeTimeProvider(UtcNow));

        var action = () => handler.HandleAsync(
            new CreateReservationCommand(EventId, 2, "Ana López", "ana@mail.com"));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateReservation_Throws_WhenHighPriceQuantityExceeded()
    {
        var @event = SampleEvent();
        var eventRepository = Substitute.For<IEventRepository>();
        eventRepository.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(@event);

        var reservationRepository = Substitute.For<IReservationRepository>();
        reservationRepository.GetByEventIdAsync(EventId, Arg.Any<CancellationToken>()).Returns([]);

        var handler = new CreateReservationCommandHandler(
            eventRepository,
            reservationRepository,
            Substitute.For<IUnitOfWork>(),
            new FakeTimeProvider(UtcNow));

        var action = () => handler.HandleAsync(
            new CreateReservationCommand(EventId, 11, "Ana López", "ana@mail.com"));

        await action.Should().ThrowAsync<InvalidReservationException>();
    }

    [Fact]
    public async Task ConfirmPayment_ThrowsConflict_WhenAlreadyConfirmed()
    {
        var @event = SampleEvent();
        var reservation = Reservation.Create(
            Guid.NewGuid(), @event, 1, "Ana López", "ana@mail.com", UtcNow, 0);
        reservation.ConfirmPayment("EV-100001", UtcNow);

        var reservationRepository = Substitute.For<IReservationRepository>();
        reservationRepository.GetByIdAsync(reservation.Id, Arg.Any<CancellationToken>()).Returns(reservation);

        var handler = new ConfirmReservationPaymentCommandHandler(
            reservationRepository,
            Substitute.For<IReservationCodeGenerator>(),
            Substitute.For<IUnitOfWork>(),
            new FakeTimeProvider(UtcNow));

        var action = () => handler.HandleAsync(new ConfirmReservationPaymentCommand(reservation.Id));

        await action.Should().ThrowAsync<ConflictException>();
    }
}
