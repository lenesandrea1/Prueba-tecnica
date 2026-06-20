using EventosVivos.Application.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Application.Events.Create;

public sealed class CreateEventCommandHandler(
    IVenueRepository venueRepository,
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<CreateEventResult> HandleAsync(
        CreateEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var venue = await venueRepository.GetByIdAsync(command.VenueId, cancellationToken);
        if (venue is null)
        {
            throw new NotFoundException($"Venue {command.VenueId} was not found.");
        }

        var overlapping = await eventRepository.GetActiveOverlappingAsync(
            command.VenueId,
            command.StartAtUtc,
            command.EndAtUtc,
            cancellationToken);

        try
        {
            var entity = Event.Create(
                Guid.NewGuid(),
                command.Title,
                command.Description,
                venue,
                command.MaxCapacity,
                command.StartAtUtc,
                command.EndAtUtc,
                command.TicketPrice,
                command.Type,
                timeProvider.GetUtcNow().UtcDateTime,
                overlapping);

            await eventRepository.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateEventResult(entity.Id, entity.Title, entity.Status);
        }
        catch (VenueScheduleConflictException ex)
        {
            throw new ConflictException(ex.Message);
        }
        catch (VenueCapacityExceededException ex)
        {
            throw new ConflictException(ex.Message);
        }
    }
}
