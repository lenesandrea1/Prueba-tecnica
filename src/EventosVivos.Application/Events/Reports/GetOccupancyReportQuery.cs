using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.Events.Reports;

public sealed record GetOccupancyReportQuery(Guid EventId);

public sealed record OccupancyReportResult(
    Guid EventId,
    string EventTitle,
    int MaxCapacity,
    int SoldTickets,
    int AvailableTickets,
    decimal OccupancyPercentage,
    decimal TotalRevenue,
    EventStatus EventStatus);
