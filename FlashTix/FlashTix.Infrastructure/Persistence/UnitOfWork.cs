using FlashTix.Application.Abstractions.Persistence;

namespace FlashTix.Infrastructure.Persistence;

internal sealed class UnitOfWork(FlashTixDbContext db) : IUnitOfWork
{
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}