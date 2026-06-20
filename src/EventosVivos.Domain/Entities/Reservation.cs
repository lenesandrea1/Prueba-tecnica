using System.Text.RegularExpressions;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Policies;

namespace EventosVivos.Domain.Entities;

public sealed partial class Reservation
{
  private static readonly Regex EmailPattern = EmailRegex();

    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public Event? Event { get; private set; }
    public int Quantity { get; private set; }
    public string BuyerName { get; private set; } = string.Empty;
    public string BuyerEmail { get; private set; } = string.Empty;
    public ReservationStatus Status { get; private set; } = ReservationStatus.PendientePago;
    public string? ConfirmationCode { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private Reservation()
    {
    }

    public static Reservation Create(
        Guid id,
        Event @event,
        int quantity,
        string buyerName,
        string buyerEmail,
        DateTime utcNow,
        int occupiedSeats)
    {
        ReservationPolicy.EnsureCanReserve(@event, quantity, utcNow);
        ReservationPolicy.EnsureCapacityAvailable(@event, quantity, occupiedSeats);
        ValidateBuyer(buyerName, buyerEmail);

        return new Reservation
        {
            Id = id,
            EventId = @event.Id,
            Event = @event,
            Quantity = quantity,
            BuyerName = buyerName.Trim(),
            BuyerEmail = buyerEmail.Trim().ToLowerInvariant(),
            Status = ReservationStatus.PendientePago,
            CreatedAtUtc = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc)
        };
    }

    public void ConfirmPayment(string confirmationCode, DateTime utcNow)
    {
        if (Status == ReservationStatus.Confirmada)
        {
            throw new InvalidReservationException("Reservation is already confirmed.");
        }

        if (Status is ReservationStatus.Cancelada or ReservationStatus.Perdida)
        {
            throw new InvalidReservationException("Cancelled reservations cannot be confirmed.");
        }

        if (string.IsNullOrWhiteSpace(confirmationCode))
        {
            throw new InvalidReservationException("Confirmation code is required.");
        }

        Status = ReservationStatus.Confirmada;
        ConfirmationCode = confirmationCode.Trim().ToUpperInvariant();
        CreatedAtUtc = DateTime.SpecifyKind(CreatedAtUtc, DateTimeKind.Utc);
        _ = utcNow;
    }

    public void Cancel(Event @event, DateTime utcNow)
    {
        if (Status is ReservationStatus.Cancelada or ReservationStatus.Perdida)
        {
            throw new InvalidReservationException("Reservation is already cancelled.");
        }

        var now = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

        if (Status == ReservationStatus.PendientePago)
        {
            Status = ReservationStatus.Cancelada;
            CancelledAtUtc = now;
            return;
        }

        if (Status == ReservationStatus.Confirmada)
        {
            if (ReservationPolicy.AppliesCancellationPenalty(@event.StartAtUtc, now))
            {
                Status = ReservationStatus.Perdida;
            }
            else
            {
                Status = ReservationStatus.Cancelada;
            }

            CancelledAtUtc = now;
        }
    }

    private static void ValidateBuyer(string buyerName, string buyerEmail)
    {
        if (string.IsNullOrWhiteSpace(buyerName) || buyerName.Trim().Length < 2)
        {
            throw new InvalidReservationException("Buyer name is required.");
        }

        if (string.IsNullOrWhiteSpace(buyerEmail) || !EmailPattern.IsMatch(buyerEmail.Trim()))
        {
            throw new InvalidReservationException("Buyer email format is invalid.");
        }
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegex();
}
