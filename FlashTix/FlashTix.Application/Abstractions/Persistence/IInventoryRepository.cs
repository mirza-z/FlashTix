namespace FlashTix.Application.Abstractions.Persistence;

public interface IInventoryRepository
{
    Task<bool> TryReserveAsync(Guid eventId, int quantity, CancellationToken cancellationToken);
}