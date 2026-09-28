using FlashTix.Application.Abstractions;

namespace FlashTix.Application.Reservations;

public static class ReservationErrors
{
    public static readonly Error EventNotFound =
        Error.NotFound("Event.NotFound", "Event does not exist or is not published.");

    public static readonly Error SaleNotStarted =
        Error.Conflict("Event.SaleNotStarted", "Ticket sale has not started yet.");

    public static readonly Error SoldOut =
        Error.Conflict("Reservation.SoldOut", "Not enough tickets available.");

    public static readonly Error PurchaseLimitExceeded =
        Error.Validation("Reservation.PurchaseLimitExceeded", "Purchase limit per user exceeded.");
}