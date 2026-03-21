using ElsaMina.Commands.VoltorbFlip;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.VoltorbFlip;

public class FlipVoltorbFlipCommandTest
{
    private IRoomsManager _roomsManager;
    private IVoltorbFlipGameManager _gameManager;
    private FlipVoltorbFlipCommand _command;
    private IContext _context;
    private IRoom _room;
    private IVoltorbFlipGame _voltorbFlipGame;
    private IUser _sender;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _gameManager = Substitute.For<IVoltorbFlipGameManager>();
        _command = new FlipVoltorbFlipCommand(_roomsManager, _gameManager);
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _voltorbFlipGame = Substitute.For<IVoltorbFlipGame>();
        _sender = Substitute.For<IUser>();

        _context.Sender.Returns(_sender);
        _room.Game.Returns(_voltorbFlipGame);
        _roomsManager.GetRoom("test-room").Returns(_room);
        _gameManager.GetGame(Arg.Any<string>(), Arg.Any<string>()).ReturnsNull();
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
    public async Task Test_RunAsync_ShouldCallFlipTile_WhenTargetIsValid()
    {
        // Arrange
        _context.Target.Returns("test-room, 2, 3");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).FlipTile(_sender, 2, 3);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenTargetHasTooFewParts()
    {
        // Arrange
        _context.Target.Returns("test-room, 2");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().FlipTile(Arg.Any<IUser>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRowIsNotAnInteger()
    {
        // Arrange
        _context.Target.Returns("test-room, abc, 3");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().FlipTile(Arg.Any<IUser>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenColIsNotAnInteger()
    {
        // Arrange
        _context.Target.Returns("test-room, 2, abc");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().FlipTile(Arg.Any<IUser>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("nonexistent-room, 2, 3");
        _roomsManager.GetRoom("nonexistent-room").ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().FlipTile(Arg.Any<IUser>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNoVoltorbFlipGame()
    {
        // Arrange
        _context.Target.Returns("test-room, 2, 3");
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().FlipTile(Arg.Any<IUser>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNullGame()
    {
        // Arrange
        _context.Target.Returns("test-room, 2, 3");
        _room.Game.ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().FlipTile(Arg.Any<IUser>(), Arg.Any<int>(), Arg.Any<int>());
    }
}
