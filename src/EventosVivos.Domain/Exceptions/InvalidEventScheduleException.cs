namespace EventosVivos.Domain.Exceptions;

public sealed class InvalidEventScheduleException(string reason)
    : DomainException(reason);
