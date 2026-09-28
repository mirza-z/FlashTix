using FlashTix.Domain.Events;

namespace FlashTix.Application.Abstractions.Persistence;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}