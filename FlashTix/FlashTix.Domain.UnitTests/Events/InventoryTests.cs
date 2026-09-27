using FlashTix.Domain.Events;
using Xunit;

namespace FlashTix.Domain.UnitTests.Events;

public class InventoryTests
{
    [Fact]
    public void Reserve_WhenEnoughAvailable_DecreasesAvailableAndIncrementsVersion()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), capacity: 100);

        inventory.Reserve(quantity: 30);

        Assert.Equal(70, inventory.Available);
        Assert.Equal(1, inventory.Version);
    }

    [Fact]
    public void Reserve_WhenNotEnoughAvailable_Throws()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), capacity: 10);

        Assert.Throws<InvalidOperationException>(() => inventory.Reserve(quantity: 11));
    }

    [Fact]
    public void Reserve_WhenQuantityIsZeroOrNegative_ThrowsArgumentException()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), capacity: 100);

        Assert.Throws<ArgumentException>(() => inventory.Reserve(quantity: 0));
    }

    [Fact]
    public void Release_WhenWithinCapacity_IncreasesAvailableAndIncrementsVersion()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), capacity: 100);

        inventory.Reserve(quantity: 30);

        inventory.Release(quantity: 30);

        Assert.Equal(100, inventory.Available);
        Assert.Equal(2, inventory.Version);
    }

    [Fact]
    public void Release_WhenQuantityIsZeroOrNegative_ThrowsArgumentException()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), capacity: 100);

        Assert.Throws<ArgumentException>(() => inventory.Release(quantity: -1));
    }

    [Fact]
    public void Release_WhenExceedsCapacity_Throws()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), capacity: 100);

        Assert.Throws<InvalidOperationException>(() => inventory.Release(quantity: 101));
    }
}