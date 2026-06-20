using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Guards;
using FluentAssertions;

namespace EventosVivos.Tests.Unit.Domain;

public sealed class EventScheduleGuardTests
{
    [Fact]
    public void EnsureWeekendNightRestriction_AllowsStartAt2200OnSaturday()
    {
        var start = new DateTime(2026, 6, 20, 22, 0, 0, DateTimeKind.Utc);

        var action = () => EventScheduleGuard.EnsureWeekendNightRestriction(start);

        action.Should().NotThrow();
    }

    [Fact]
    public void EnsureWeekendNightRestriction_RejectsStartAfter2200OnSunday()
    {
        var start = new DateTime(2026, 6, 21, 22, 1, 0, DateTimeKind.Utc);

        var action = () => EventScheduleGuard.EnsureWeekendNightRestriction(start);

        action.Should().Throw<InvalidEventScheduleException>()
            .WithMessage("*22:00*");
    }

    [Fact]
    public void EnsureWeekendNightRestriction_AllowsWeekdayLateStart()
    {
        var start = new DateTime(2026, 6, 19, 23, 0, 0, DateTimeKind.Utc);

        var action = () => EventScheduleGuard.EnsureWeekendNightRestriction(start);

        action.Should().NotThrow();
    }

    [Fact]
    public void RangesOverlap_DetectsPartialOverlap()
    {
        var startA = new DateTime(2026, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var endA = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);
        var startB = new DateTime(2026, 1, 10, 11, 0, 0, DateTimeKind.Utc);
        var endB = new DateTime(2026, 1, 10, 13, 0, 0, DateTimeKind.Utc);

        EventScheduleGuard.RangesOverlap(startA, endA, startB, endB).Should().BeTrue();
    }

    [Fact]
    public void RangesOverlap_AllowsAdjacentRanges()
    {
        var startA = new DateTime(2026, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var endA = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);
        var startB = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);
        var endB = new DateTime(2026, 1, 10, 14, 0, 0, DateTimeKind.Utc);

        EventScheduleGuard.RangesOverlap(startA, endA, startB, endB).Should().BeFalse();
    }
}

public sealed class EventCreateTests
{
    private static readonly Venue Auditorium = new(1, "Auditorio Central", 200, "Bogotá");
    private static readonly DateTime UtcNow = new(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_RejectsCapacityAboveVenue()
    {
        var action = () => Event.Create(
            Guid.NewGuid(),
            "Conferencia Anual",
            "Descripción válida para la prueba de capacidad.",
            Auditorium,
            250,
            UtcNow.AddDays(10),
            UtcNow.AddDays(10).AddHours(4),
            80m,
            EventType.Conferencia,
            UtcNow,
            []);

        action.Should().Throw<VenueCapacityExceededException>();
    }

    [Fact]
    public void Create_RejectsOverlappingActiveEventsAtSameVenue()
    {
        var existing = Event.Create(
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

        var action = () => Event.Create(
            Guid.NewGuid(),
            "Nuevo evento",
            "Descripción del nuevo evento con conflicto.",
            Auditorium,
            100,
            UtcNow.AddDays(5).AddHours(1),
            UtcNow.AddDays(5).AddHours(4),
            60m,
            EventType.Conferencia,
            UtcNow,
            [existing]);

        action.Should().Throw<VenueScheduleConflictException>();
    }

    [Fact]
    public void Create_AllowsValidEvent()
    {
        var entity = Event.Create(
            Guid.NewGuid(),
            "Concierto de prueba",
            "Descripción válida para el concierto de prueba.",
            Auditorium,
            150,
            UtcNow.AddDays(20),
            UtcNow.AddDays(20).AddHours(2),
            99.5m,
            EventType.Concierto,
            UtcNow,
            []);

        entity.Status.Should().Be(EventStatus.Activo);
        entity.MaxCapacity.Should().Be(150);
    }
}
