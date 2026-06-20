namespace EventosVivos.Application.Common;

public interface IReservationCodeGenerator
{
    Task<string> GenerateUniqueAsync(CancellationToken cancellationToken = default);
}
