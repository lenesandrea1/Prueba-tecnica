using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Domain.Guards;

public static class EventScheduleGuard
{
    private static readonly TimeSpan WeekendLatestStart = new(22, 0, 0);

    public static void EnsureWeekendNightRestriction(DateTime startAtUtc)
    {
        var start = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);

        if (start.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
        {
            return;
        }

        if (start.TimeOfDay > WeekendLatestStart)
        {
            throw new InvalidEventScheduleException(
                "Weekend events cannot start after 22:00 UTC.");
        }
    }

    public static void EnsureNoVenueOverlap(
        int venueId,
        DateTime startAtUtc,
        DateTime endAtUtc,
        IReadOnlyCollection<Event> overlappingCandidates)
    {
        var start = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);

        foreach (var candidate in overlappingCandidates)
        {
            if (candidate.VenueId != venueId)
            {
                continue;
            }

            if (RangesOverlap(start, end, candidate.StartAtUtc, candidate.EndAtUtc))
            {
                throw new VenueScheduleConflictException(venueId, start, end);
            }
        }
    }

    public static bool RangesOverlap(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
    {
        return startA < endB && startB < endA;
    }
}
