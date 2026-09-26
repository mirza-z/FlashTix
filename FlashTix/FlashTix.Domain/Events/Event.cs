namespace FlashTix.Domain.Events;

public enum EventStatus
{
    Draft,
    Published,
    Cancelled
}

public class Event
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Venue { get; private set; } = null!;
    public DateTimeOffset StartsAt { get; private set; }
    public long PriceMinorUnits { get; private set; }   
    public int Capacity { get; private set; }
    public DateTimeOffset SaleStartsAt { get; private set; }
    public EventStatus Status { get; private set; }
    public string? ImageUrl { get; private set; }

    private Event() { } 

    public static Event Create(string title, string venue, DateTimeOffset startsAt,
        long priceMinorUnits, int capacity, DateTimeOffset saleStartsAt, string? imageUrl)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be positive.", nameof(capacity));

        return new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Venue = venue,
            StartsAt = startsAt,
            PriceMinorUnits = priceMinorUnits,
            Capacity = capacity,
            SaleStartsAt = saleStartsAt,
            Status = EventStatus.Draft,
            ImageUrl = imageUrl
        };
    }

    public void Publish()
    {
        if (Status != EventStatus.Draft)
            throw new InvalidOperationException($"Cannot publish event in status {Status}.");
        Status = EventStatus.Published;
    }

    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
            throw new InvalidOperationException("Event already cancelled.");
        Status = EventStatus.Cancelled;
    }

    public void UpdateCapacity(int newCapacity, int currentSoldPlusHeld)
    {
        if (newCapacity < currentSoldPlusHeld)
            throw new InvalidOperationException(
                $"Cannot reduce capacity below sold+held ({currentSoldPlusHeld}).");
        Capacity = newCapacity;
    }
}