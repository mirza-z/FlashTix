namespace FlashTix.Domain.Reservations;

public enum ReservationStatus
{
    Held,
    Confirmed,
    Expired,
    Cancelled,
    Refunded
}

public class Reservation
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public int Quantity { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string? PaymentIntentId { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;

    private Reservation() { } 

    public static Reservation CreateHeld(Guid eventId, Guid userId, int quantity,
        string idempotencyKey, TimeSpan holdDuration, DateTimeOffset now)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        return new Reservation
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            UserId = userId,
            Quantity = quantity,
            Status = ReservationStatus.Held,
            ExpiresAt = now.Add(holdDuration),
            CreatedAt = now,
            IdempotencyKey = idempotencyKey
        };
    }

    public void AttachPaymentIntent(string paymentIntentId)
    {
        EnsureStatus(ReservationStatus.Held, nameof(AttachPaymentIntent));
        PaymentIntentId = paymentIntentId;
    }

    public void Confirm()
    {
        EnsureStatus(ReservationStatus.Held, nameof(Confirm));
        Status = ReservationStatus.Confirmed;
    }

    public void Expire()
    {
        EnsureStatus(ReservationStatus.Held, nameof(Expire));
        Status = ReservationStatus.Expired;
    }

    public void Cancel()
    {
        EnsureStatus(ReservationStatus.Held, nameof(Cancel));
        Status = ReservationStatus.Cancelled;
    }

    public void Refund()
    {
        EnsureStatus(ReservationStatus.Confirmed, nameof(Refund));
        Status = ReservationStatus.Refunded;
    }

    public bool IsExpired(DateTimeOffset now) =>
        Status == ReservationStatus.Held && ExpiresAt < now;

    private void EnsureStatus(ReservationStatus required, string operation)
    {
        if (Status != required)
            throw new InvalidOperationException(
                $"Cannot perform '{operation}': reservation is {Status}, expected {required}.");
    }
}