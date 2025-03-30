using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.ConnectFour;

public class EndConnectFourTest
{
    private EndConnectFour _command;
    private IContext _context;
    private IRoom _room;
    private IConnectFourGame _connectFourGame;

    [SetUp]
    public void SetUp()
    {
        _command = new EndConnectFour();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _connectFourGame = Substitute.For<IConnectFourGame>();
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public async Task Test_RunAsync_ShouldCancelGameAndReplyCancelledMessage_WhenGameIsConnectFour()
    {
        // Arrange
        _context.Room.Returns(_room);
        _room.Game.Returns(_connectFourGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _connectFourGame.Received(1).Cancel();
        _context.Received(1).ReplyLocalizedMessage("c4_game_cancelled");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyOngoingGameMessage_WhenNoConnectFourGameExists()
    {
        // Arrange
        _context.Room.Returns(_room);
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("c4_game_ongoing_game");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyOngoingGameMessage_WhenRoomIsNull()
    {
        // Arrange
        _context.Room.ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("c4_game_ongoing_game");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyOngoingGameMessage_WhenRoomHasNoGame()
    {
        // Arrange
        _context.Room.Returns(_room);
        _room.Game.ReturnsNull();

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("c4_game_ongoing_game");
    }
}