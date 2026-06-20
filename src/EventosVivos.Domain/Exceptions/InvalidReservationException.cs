namespace EventosVivos.Domain.Exceptions;

public sealed class InvalidReservationException : DomainException
{
    public InvalidReservationException(string message) : base(message)
    {
    }
}
