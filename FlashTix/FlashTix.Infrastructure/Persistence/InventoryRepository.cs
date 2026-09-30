using FlashTix.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashTix.Infrastructure.Persistence;

internal sealed class InventoryRepository(FlashTixDbContext db) : IInventoryRepository
{
    public async Task<bool> TryReserveAsync(Guid eventId, int quantity, CancellationToken cancellationToken)
    {
        var affected = await db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE inventory
            SET available = available - {quantity}, version = version + 1
            WHERE event_id = {eventId} AND available >= {quantity}
            """, cancellationToken);

        return affected == 1;
    }
}