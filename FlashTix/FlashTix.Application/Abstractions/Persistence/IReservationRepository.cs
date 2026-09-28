using FlashTix.Domain.Reservations;

namespace FlashTix.Application.Abstractions.Persistence;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken);

    Task<int> GetActiveQuantityForUserAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
}