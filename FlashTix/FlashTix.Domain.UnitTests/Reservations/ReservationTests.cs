using FlashTix.Domain.Reservations;

namespace FlashTix.Domain.UnitTests.Reservations;

public class ReservationTests
{
    [Fact]
    public void Confirm_WhenHeld_TransitionsToConfirmed()
    {
        var reservation = Reservation.CreateHeld(
            eventId: Guid.NewGuid(),
            userId: Guid.NewGuid(),
            quantity: 2,
            idempotencyKey: "key-1",
            holdDuration: TimeSpan.FromMinutes(10),
            now: DateTimeOffset.UtcNow);

        reservation.Confirm();

        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void Confirm_WhenAlreadyExpired_Throws()
    {

        var reservation = Reservation.CreateHeld(
            Guid.NewGuid(), Guid.NewGuid(), 2, "key-2",
            TimeSpan.FromMinutes(10), DateTimeOffset.UtcNow);
        reservation.Expire(); 

        Assert.Throws<InvalidOperationException>(() => reservation.Confirm());
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_Throws()
    {

        var reservation = Reservation.CreateHeld(
            Guid.NewGuid(), Guid.NewGuid(), 2, "key-3",
            TimeSpan.FromMinutes(10), DateTimeOffset.UtcNow);
        reservation.Confirm(); 

        Assert.Throws<InvalidOperationException>(() => reservation.Confirm());
    }

    [Fact]
    public void Expire_WhenHeld_TransitionsToExpired()
    {
        var reservation = Reservation.CreateHeld(
          Guid.NewGuid(), Guid.NewGuid(), 2, "key-2",
          TimeSpan.FromMinutes(10), DateTimeOffset.UtcNow);
        reservation.Expire();

        Assert.Equal(ReservationStatus.Expired, reservation.Status);
    }

    [Fact]
    public void Expire_WhenAlreadyConfirmed_Throws()
    {
        var reservation = Reservation.CreateHeld(
            Guid.NewGuid(), Guid.NewGuid(), 2, "key-3",
            TimeSpan.FromMinutes(10), DateTimeOffset.UtcNow);
        reservation.Confirm();

        Assert.Throws<InvalidOperationException>(() => reservation.Expire());
    }
}