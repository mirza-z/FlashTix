using FlashTix.Application.Abstractions.Persistence;
using FlashTix.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace FlashTix.Infrastructure.Persistence;

internal sealed class ReservationRepository(FlashTixDbContext db, TimeProvider timeProvider)
    : IReservationRepository
{
    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken) =>
        await db.Reservations.AddAsync(reservation, cancellationToken);

    public async Task<int> GetActiveQuantityForUserAsync(
        Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();

        return await db.Reservations
            .Where(r => r.EventId == eventId && r.UserId == userId &&
                        (r.Status == ReservationStatus.Confirmed ||
                         (r.Status == ReservationStatus.Held && r.ExpiresAt > now)))
            .SumAsync(r => r.Quantity, cancellationToken);
    }
}