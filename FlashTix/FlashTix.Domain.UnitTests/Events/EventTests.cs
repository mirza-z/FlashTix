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
}