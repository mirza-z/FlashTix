namespace FlashTix.Application.Reservations.Commands.HoldTickets;

public sealed record HoldTicketsResult(
    Guid ReservationId,
    DateTimeOffset ExpiresAt
);