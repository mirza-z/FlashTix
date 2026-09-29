using FlashTix.Application.Abstractions;
using FlashTix.Application.Abstractions.Messaging;
using FlashTix.Application.Abstractions.Persistence;
using FlashTix.Domain.Events;
using FlashTix.Domain.Reservations;

namespace FlashTix.Application.Reservations.Commands.HoldTickets;

public sealed class HoldTicketsCommandHandler(
    IEventRepository events,
    IInventoryRepository inventory,
    IReservationRepository reservations,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ReservationOptions options)
    : ICommandHandler<HoldTicketsCommand, HoldTicketsResult>
{
    public async Task<Result<HoldTicketsResult>> Handle(
        HoldTicketsCommand command, CancellationToken cancellationToken)
    {
        if (command.Quantity <= 0)
            return ReservationErrors.InvalidQuantity;

        var ev = await events.GetByIdAsync(command.EventId, cancellationToken);
        if (ev is null || ev.Status != EventStatus.Published)
            return ReservationErrors.EventNotFound;

        var now = timeProvider.GetUtcNow();
        if (now < ev.SaleStartsAt)
            return ReservationErrors.SaleNotStarted;

        var alreadyHeld = await reservations.GetActiveQuantityForUserAsync(
            command.EventId, command.UserId, cancellationToken);
        if (alreadyHeld + command.Quantity > options.MaxTicketsPerUser)
            return ReservationErrors.PurchaseLimitExceeded;

        return await unitOfWork.ExecuteInTransactionAsync<Result<HoldTicketsResult>>(async () =>
        {
            var reserved = await inventory.TryReserveAsync(
                command.EventId, command.Quantity, cancellationToken);
            if (!reserved)
                return ReservationErrors.SoldOut;

            var reservation = Reservation.CreateHeld(
                command.EventId, command.UserId, command.Quantity,
                command.IdempotencyKey, options.HoldDuration, now);

            await reservations.AddAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new HoldTicketsResult(reservation.Id, reservation.ExpiresAt);
        }, cancellationToken);
    }
}