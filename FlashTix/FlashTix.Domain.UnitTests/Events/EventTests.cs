using FlashTix.Domain.Events;

namespace FlashTix.Domain.UnitTests.Events;

public class EventTests
{
    [Fact]
    public void Publish_WhenDraft_TransitionsToPublished()
    {
        var ev = Event.Create("Concert", "Venue", DateTimeOffset.UtcNow.AddDays(30),
            5000, 100, DateTimeOffset.UtcNow, null);
        ev.Publish();

        Assert.Equal(EventStatus.Published, ev.Status);
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_Throws()
    {
        var ev = Event.Create("Concert", "Venue", DateTimeOffset.UtcNow.AddDays(30),
            5000, 100, DateTimeOffset.UtcNow, null);
        ev.Publish(); 

 
        Assert.Throws<InvalidOperationException>(() => ev.Publish());
    }

    [Fact]
    public void Cancel_WhenDraft_TransitionsToCancelled()
    {
        var ev = Event.Create("Concert", "Venue", DateTimeOffset.UtcNow.AddDays(30),
             5000, 100, DateTimeOffset.UtcNow, null);
        ev.Cancel();

        Assert.Equal(EventStatus.Cancelled, ev.Status);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_Throws()
    {
        var ev = Event.Create("Concert", "Venue", DateTimeOffset.UtcNow.AddDays(30),
           5000, 100, DateTimeOffset.UtcNow, null);
        ev.Cancel();

        Assert.Throws<InvalidOperationException>(() => ev.Cancel());
    }

    [Fact]
    public void UpdateCapacity_WhenNewCapacityAboveSoldPlusHeld_UpdatesCapacity()
    {
        var ev = Event.Create("Concert", "Venue", DateTimeOffset.UtcNow.AddDays(30),
            5000, 100, DateTimeOffset.UtcNow, null);

        ev.UpdateCapacity(newCapacity: 150, currentSoldPlusHeld: 80);

        Assert.Equal(150, ev.Capacity);
    }

    [Fact]
    public void UpdateCapacity_WhenNewCapacityBelowSoldPlusHeld_Throws()
    {
        var ev = Event.Create("Concert", "Venue", DateTimeOffset.UtcNow.AddDays(30),
            5000, 100, DateTimeOffset.UtcNow, null);
 
        Assert.Throws<InvalidOperationException>(() => ev.UpdateCapacity(newCapacity: 50, currentSoldPlusHeld: 80));
    }
}