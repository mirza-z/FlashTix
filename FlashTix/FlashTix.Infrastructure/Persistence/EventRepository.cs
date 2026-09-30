using FlashTix.Application.Abstractions.Persistence;
using FlashTix.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace FlashTix.Infrastructure.Persistence;

internal sealed class EventRepository(FlashTixDbContext db) : IEventRepository
{
    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
}