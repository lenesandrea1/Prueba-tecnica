using EventosVivos.Application.Common;
using EventosVivos.Application.Events.Create;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace EventosVivos.Tests.Unit.Application;

public sealed class CreateEventCommandHandlerTests
{
    private static readonly Venue Auditorium = new(1, "Auditorio Central", 200, "Bogotá");
    private static readonly DateTime UtcNow = new(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task HandleAsync_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        var venueRepository = Substitute.For<IVenueRepository>();
        venueRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Venue?)null);

        var handler = CreateHandler(venueRepository);

        var action = () => handler.HandleAsync(new CreateEventCommand(
            "Conferencia Anual",
            "Descripción válida para la prueba.",
            99,
            100,
            UtcNow.AddDays(3),
            UtcNow.AddDays(3).AddHours(2),
            50m,
            EventType.Conferencia));

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsConflict_WhenScheduleOverlaps()
    {
        var venueRepository = Substitute.For<IVenueRepository>();
        venueRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Auditorium);

        var overlapping = Event.Create(
            Guid.NewGuid(),
            "Evento existente",
            "Descripción del evento existente en el venue.",
            Auditorium,
            100,
            UtcNow.AddDays(5),
            UtcNow.AddDays(5).AddHours(3),
            50m,
            EventType.Taller,
            UtcNow,
            []);

        var eventRepository = Substitute.For<IEventRepository>();
        eventRepository.GetActiveOverlappingAsync(1, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns([overlapping]);

        var handler = CreateHandler(venueRepository, eventRepository);

        var action = () => handler.HandleAsync(new CreateEventCommand(
            "Nuevo evento",
            "Descripción del nuevo evento con conflicto.",
            1,
            100,
            UtcNow.AddDays(5).AddHours(1),
            UtcNow.AddDays(5).AddHours(4),
            60m,
            EventType.Conferencia));

        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task HandleAsync_PersistsEvent_WhenRequestIsValid()
    {
        var venueRepository = Substitute.For<IVenueRepository>();
        venueRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Auditorium);

        var eventRepository = Substitute.For<IEventRepository>();
        eventRepository.GetActiveOverlappingAsync(1, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = CreateHandler(venueRepository, eventRepository, unitOfWork);

        var result = await handler.HandleAsync(new CreateEventCommand(
            "Conferencia Anual",
            "Descripción válida para la prueba de creación.",
            1,
            120,
            UtcNow.AddDays(8),
            UtcNow.AddDays(8).AddHours(5),
            110m,
            EventType.Conferencia));

        result.Title.Should().Be("Conferencia Anual");
        result.Status.Should().Be(EventStatus.Activo);
        await eventRepository.Received(1).AddAsync(Arg.Any<Event>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static CreateEventCommandHandler CreateHandler(
        IVenueRepository? venueRepository = null,
        IEventRepository? eventRepository = null,
        IUnitOfWork? unitOfWork = null)
    {
        return new CreateEventCommandHandler(
            venueRepository ?? Substitute.For<IVenueRepository>(),
            eventRepository ?? Substitute.For<IEventRepository>(),
            unitOfWork ?? Substitute.For<IUnitOfWork>(),
            new FakeTimeProvider(UtcNow));
    }
}
