namespace FlashTix.Application.Reservations;

public sealed class ReservationOptions
{
    public TimeSpan HoldDuration { get; init; } = TimeSpan.FromMinutes(10);
    public int MaxTicketsPerUser { get; init; } = 4;
}