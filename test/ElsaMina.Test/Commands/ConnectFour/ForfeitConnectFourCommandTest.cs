using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.Test.Commands.ConnectFour;

public class ForfeitConnectFourCommandTest
{
    private ForfeitConnectFourCommand _command;
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

        _command = new ForfeitConnectFourCommand(_roomsManager);
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
    public async Task Test_Run_ShouldNotCallForfeit_WhenRoomDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("room1");
        _roomsManager.GetRoom("room1").Returns((IRoom)null);

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.DidNotReceive().Forfeit(Arg.Any<IUser>());
    }

    [Test]
    public async Task Test_Run_ShouldNotCallForfeit_WhenRoomHasNoGame()
    {
        // Arrange
        _context.Target.Returns("room1");
        _room.Game.Returns((IGame)null);
        _roomsManager.GetRoom("room1").Returns(_room);

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.DidNotReceive().Forfeit(Arg.Any<IUser>());
    }

    [Test]
    public async Task Test_Run_ShouldNotCallForfeit_WhenGameIsNotConnectFour()
    {
        // Arrange
        var otherGame = Substitute.For<IGame>(); // Not IConnectFourGame
        _context.Target.Returns("room1");
        _room.Game.Returns(otherGame);
        _roomsManager.GetRoom("room1").Returns(_room);

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.DidNotReceive().Forfeit(Arg.Any<IUser>());
    }

    [Test]
    public async Task Test_Run_ShouldCallForfeit_WhenGameIsConnectFour()
    {
        // Arrange
        _context.Target.Returns("room1");
        _room.Game.Returns(_connectFourGame);
        _roomsManager.GetRoom("room1").Returns(_room);
        var sender = Substitute.For<IUser>();
        _context.Sender.Returns(sender);

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.Received(1).Forfeit(sender);
    }
}