using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.ConnectFour;

public class PlayConnectFourCommandTest
{
    private PlayConnectFourCommand _command;
    private IRoomsManager _roomsManager;
    private IContext _context;
    private IRoom _room;
    private IConnectFourGame _connectFourGame;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _connectFourGame = Substitute.For<IConnectFourGame>();

        _command = new PlayConnectFourCommand(_roomsManager);
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
    public async Task Test_RunAsync_ShouldMakeMove_WhenGameIsConnectFour()
    {
        // Arrange
        _context.Target.Returns("room1, move1");
        _roomsManager.GetRoom("room1").Returns(_room);
        _room.Game.Returns(_connectFourGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _connectFourGame.Received(1).Play(_context.Sender, "move1");
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("room1, move1");
        _roomsManager.GetRoom("room1").ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _connectFourGame.DidNotReceive().Play(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenRoomHasNoGame()
    {
        // Arrange
        _context.Target.Returns("room1, move1");
        _roomsManager.GetRoom("room1").Returns(_room);
        _room.Game.Returns((IGame)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _connectFourGame.DidNotReceive().Play(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldDoNothing_WhenGameIsNotConnectFour()
    {
        // Arrange
        _context.Target.Returns("room1, move1");
        _roomsManager.GetRoom("room1").Returns(_room);
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _connectFourGame.DidNotReceive().Play(Arg.Any<IUser>(), Arg.Any<string>());
    }

    [Test]
    public void Test_RunAsync_ShouldThrowException_WhenTargetIsInvalid()
    {
        // Arrange
        _context.Target.Returns("invalid_input");

        // Act & Assert
        Assert.ThrowsAsync<IndexOutOfRangeException>(() => _command.RunAsync(_context));
    }
}