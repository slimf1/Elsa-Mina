using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Test.Fixtures;
using NSubstitute;

namespace ElsaMina.Test.Commands.ConnectFour;

public class JoinConnectFourCommandTests
{
    private IRoomsManager _roomsManager;
    private IContext _context;
    private JoinConnectFourCommand _command;
    private IConnectFourGame _connectFourGame;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _context = Substitute.For<IContext>();
        _connectFourGame = Substitute.For<IConnectFourGame>();

        _command = new JoinConnectFourCommand(_roomsManager);
    }

    [Test]
    public async Task Test_Run_ShouldJoinGame_WhenRoomExistsAndHasConnectFourGame()
    {
        // Arrange
        var roomId = "room1";
        var room = Substitute.For<IRoom>();
        room.Game.Returns(_connectFourGame);
        _roomsManager.GetRoom(roomId).Returns(room);
        _context.Target.Returns(roomId);
        var sender = UserFixtures.AdminUser("player1");
        _context.Sender.Returns(sender);

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.Received(1).JoinGame(sender);
    }

    [Test]
    public async Task Test_Run_ShouldNotJoinGame_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = "room1";
        var room = Substitute.For<IRoom>();
        room.Game.Returns(_connectFourGame);
        _roomsManager.GetRoom(roomId).Returns((Room)null);
        _context.Target.Returns(roomId);
        _context.Sender.Returns(UserFixtures.AdminUser("player1"));

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.DidNotReceive().JoinGame(_context.Sender);
    }

    [Test]
    [TestCase(false, 1)]
    [TestCase(true, 0)]
    public async Task Test_Run_ShouldDisplayAnnounce_WhenGameIsNotStarted(bool isStarted, int expectedAnnounces)
    {
        // Arrange
        var roomId = "room1";
        var room = Substitute.For<IRoom>();
        room.Game.Returns(_connectFourGame);
        _roomsManager.GetRoom(roomId).Returns(room);
        _context.Target.Returns(roomId);
        _context.Sender.Returns(UserFixtures.AdminUser("player1"));
        _connectFourGame.IsStarted.Returns(isStarted);

        // Act
        await _command.Run(_context);

        // Assert
        await _connectFourGame.Received(expectedAnnounces).DisplayAnnounce();
    }
}