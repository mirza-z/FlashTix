namespace FlashTix.Domain.Events;

public class Inventory
{
    public Guid EventId { get; private set; }
    public int Available { get; private set; }
    public int Version { get; private set; } 

    private Inventory() { } 

    public static Inventory Create(Guid eventId, int capacity)
        => new() { EventId = eventId, Available = capacity, Version = 0 };

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        if (Available < quantity)
            throw new InvalidOperationException("Not enough tickets available.");

        Available -= quantity;
        Version++;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        Available += quantity;
        Version++;
    }
}