using ElsaMina.Commands.VoltorbFlip;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.VoltorbFlip;

public class ToggleMarkVoltorbFlipCommandTest
{
    private IRoomsManager _roomsManager;
    private IVoltorbFlipGameManager _gameManager;
    private ToggleMarkVoltorbFlipCommand _command;
    private IContext _context;
    private IRoom _room;
    private IVoltorbFlipGame _voltorbFlipGame;
    private IUser _sender;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _gameManager = Substitute.For<IVoltorbFlipGameManager>();
        _command = new ToggleMarkVoltorbFlipCommand(_roomsManager, _gameManager);
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _voltorbFlipGame = Substitute.For<IVoltorbFlipGame>();
        _sender = Substitute.For<IUser>();

        _context.Sender.Returns(_sender);
        _context.Target.Returns("test-room, 1");
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
    public async Task Test_RunAsync_ShouldCallSetMarkerType_WhenVoltorbFlipGameExists()
    {
        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).SetMarkerType(_sender, VoltorbFlipMarkerType.Voltorb);
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimRoomId()
    {
        // Arrange
        _context.Target.Returns("  test-room  , 1");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).SetMarkerType(_sender, VoltorbFlipMarkerType.Voltorb);
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCorrectMarkerType_WhenMarkerTypeIsOne()
    {
        // Arrange
        _context.Target.Returns("test-room, 2");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).SetMarkerType(_sender, VoltorbFlipMarkerType.One);
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCorrectMarkerType_WhenMarkerTypeIsTwo()
    {
        // Arrange
        _context.Target.Returns("test-room, 3");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).SetMarkerType(_sender, VoltorbFlipMarkerType.Two);
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCorrectMarkerType_WhenMarkerTypeIsThree()
    {
        // Arrange
        _context.Target.Returns("test-room, 4");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).SetMarkerType(_sender, VoltorbFlipMarkerType.Three);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenMarkerTypeIsMissing()
    {
        // Arrange
        _context.Target.Returns("test-room");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenMarkerTypeIsZero()
    {
        // Arrange
        _context.Target.Returns("test-room, 0");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenMarkerTypeIsOutOfRange()
    {
        // Arrange
        _context.Target.Returns("test-room, 99");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenMarkerTypeIsNotNumeric()
    {
        // Arrange
        _context.Target.Returns("test-room, abc");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallSetMarkerType_WhenGameFoundViaGameManager()
    {
        // Arrange
        _sender.UserId.Returns("testplayer");
        _gameManager.GetGame("test-room", "testplayer").Returns(_voltorbFlipGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.Received(1).SetMarkerType(_sender, VoltorbFlipMarkerType.Voltorb);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomDoesNotExist()
    {
        // Arrange
        _roomsManager.GetRoom("test-room").ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNoVoltorbFlipGame()
    {
        // Arrange
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNullGame()
    {
        // Arrange
        _room.Game.ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _voltorbFlipGame.DidNotReceive().SetMarkerType(Arg.Any<IUser>(), Arg.Any<VoltorbFlipMarkerType>());
    }
}
