using FlashTix.Application.Abstractions;
using FlashTix.Application.Abstractions.Persistence;
using FlashTix.Application.Reservations;
using FlashTix.Application.Reservations.Commands.HoldTickets;
using FlashTix.Domain.Events;
using FlashTix.Domain.Reservations;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace FlashTix.Application.UnitTests.Reservations;

public class HoldTicketsCommandHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly IEventRepository _events = Substitute.For<IEventRepository>();
    private readonly IInventoryRepository _inventory = Substitute.For<IInventoryRepository>();
    private readonly IReservationRepository _reservations = Substitute.For<IReservationRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly FakeTimeProvider _time = new(Now);
    private readonly ReservationOptions _options = new()
    {
        MaxTicketsPerUser = 4,
        HoldDuration = TimeSpan.FromMinutes(10)
    };
    private readonly HoldTicketsCommandHandler _sut;

    public HoldTicketsCommandHandlerTests()
    {
        _uow.ExecuteInTransactionAsync(
                Arg.Any<Func<Task<Result<HoldTicketsResult>>>>(),
                Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<Task<Result<HoldTicketsResult>>>>()());

        _sut = new HoldTicketsCommandHandler(
            _events, _inventory, _reservations, _uow, _time, _options);
    }

    private static Event CreateEvent(bool publish = true, TimeSpan? saleStartsIn = null)
    {
        var ev = Event.Create(
            "Koncert", "Arena", Now.AddDays(30), 5000, 100,
            Now + (saleStartsIn ?? TimeSpan.FromHours(-1)), null);
        if (publish) ev.Publish();
        return ev;
    }

    private static HoldTicketsCommand CreateCommand(Guid eventId, int quantity = 2) =>
        new(eventId, Guid.NewGuid(), quantity, "idem-key-1");

    private void SetupEvent(Event ev) =>
        _events.GetByIdAsync(ev.Id, Arg.Any<CancellationToken>()).Returns(ev);

    private void SetupAlreadyHeld(int quantity) =>
        _reservations.GetActiveQuantityForUserAsync(
                Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(quantity);

    private void SetupTryReserve(bool result) =>
        _inventory.TryReserveAsync(
                Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(result);

    private void AssertNothingWritten()
    {
        _inventory.DidNotReceiveWithAnyArgs().TryReserveAsync(default, default, default);
        _reservations.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        _uow.DidNotReceiveWithAnyArgs()
            .ExecuteInTransactionAsync<Result<HoldTicketsResult>>(default!, default);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsReservation()
    {
        var ev = CreateEvent();
        SetupEvent(ev);
        SetupAlreadyHeld(0);
        SetupTryReserve(true);
        var command = CreateCommand(ev.Id, quantity: 2);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Now + _options.HoldDuration, result.Value.ExpiresAt);
        await _reservations.Received(1).AddAsync(
            Arg.Is<Reservation>(r =>
                r.EventId == ev.Id &&
                r.UserId == command.UserId &&
                r.Quantity == 2),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_QuantityZeroOrNegative_ReturnsInvalidQuantity(int quantity)
    {
        var command = CreateCommand(Guid.NewGuid(), quantity);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.InvalidQuantity, result.Error);
        await _events.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
        AssertNothingWritten();
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsEventNotFound()
    {
        var command = CreateCommand(Guid.NewGuid());
        _events.GetByIdAsync(command.EventId, Arg.Any<CancellationToken>())
            .Returns((Event?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.EventNotFound, result.Error);
        AssertNothingWritten();
    }

    [Fact]
    public async Task Handle_EventNotPublished_ReturnsEventNotFound()
    {
        var ev = CreateEvent(publish: false); 
        SetupEvent(ev);
        var command = CreateCommand(ev.Id);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.EventNotFound, result.Error);
        AssertNothingWritten();
    }

    [Fact]
    public async Task Handle_SaleNotStarted_ReturnsSaleNotStarted()
    {
        var ev = CreateEvent(saleStartsIn: TimeSpan.FromHours(1));
        SetupEvent(ev);
        var command = CreateCommand(ev.Id);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.SaleNotStarted, result.Error);
        AssertNothingWritten();
    }

    [Fact]
    public async Task Handle_LimitExceeded_ReturnsPurchaseLimitExceeded()
    {
        var ev = CreateEvent();
        SetupEvent(ev);
        SetupAlreadyHeld(3);              
        var command = CreateCommand(ev.Id, quantity: 2);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.PurchaseLimitExceeded, result.Error);
        AssertNothingWritten();
    }

    [Fact]
    public async Task Handle_SoldOut_ReturnsSoldOut()
    {
        var ev = CreateEvent();
        SetupEvent(ev);
        SetupAlreadyHeld(0);
        SetupTryReserve(false);
        var command = CreateCommand(ev.Id);

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.SoldOut, result.Error);
        await _reservations.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await _uow.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ExactlyAtLimit_Succeeds()
    {
        var ev = CreateEvent();
        SetupEvent(ev);
        SetupAlreadyHeld(2);                
        SetupTryReserve(true);

        var result = await _sut.Handle(CreateCommand(ev.Id, quantity: 2), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}