using FlashTix.Application.Abstractions.Messaging;

namespace FlashTix.Application.Reservations.Commands.HoldTickets;

public sealed record HoldTicketsCommand(
    Guid EventId,
    Guid UserId,
    int Quantity,
    string IdempotencyKey
) : ICommand<HoldTicketsResult>;