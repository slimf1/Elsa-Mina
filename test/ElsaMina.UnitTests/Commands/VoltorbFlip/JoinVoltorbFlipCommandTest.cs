using ElsaMina.Commands.VoltorbFlip;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.VoltorbFlip;

public class JoinVoltorbFlipCommandTest
{
    private IRoomsManager _roomsManager;
    private JoinVoltorbFlipCommand _command;
    private IContext _context;
    private IRoom _room;
    private IVoltorbFlipGame _voltorbFlipGame;
    private IUser _sender;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _command = new JoinVoltorbFlipCommand(_roomsManager);
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _voltorbFlipGame = Substitute.For<IVoltorbFlipGame>();
        _sender = Substitute.For<IUser>();

        _context.Sender.Returns(_sender);
        _context.Target.Returns("test-room");
        _voltorbFlipGame.IsStarted.Returns(false);
        _room.Game.Returns(_voltorbFlipGame);
        _roomsManager.GetRoom("test-room").Returns(_room);
    }

    [Test]
    public void Test_IsPrivateMessageOnly_ShouldBeTrue()
    {
        Assert.That(_command.IsPrivateMessageOnly, Is.True);
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetOwnerAndStartRound_WhenGameIsWaiting()
    {
        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(_voltorbFlipGame.Owner, Is.SameAs(_sender));
        await _voltorbFlipGame.Received(1).StartNewRound();
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimRoomId()
    {
        // Arrange
        _context.Target.Returns("  test-room  ");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).StartNewRound();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenGameIsAlreadyStarted()
    {
        // Arrange
        _voltorbFlipGame.IsStarted.Returns(true);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().StartNewRound();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomDoesNotExist()
    {
        // Arrange
        _roomsManager.GetRoom("test-room").ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().StartNewRound();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNoVoltorbFlipGame()
    {
        // Arrange
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().StartNewRound();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNullGame()
    {
        // Arrange
        _room.Game.ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().StartNewRound();
    }
}
