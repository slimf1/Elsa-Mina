using ElsaMina.Commands.GuessingGame;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.GuessingGame;

[TestFixture]
public class EndGuessingGameCommandTest
{
    private IContext _context;
    private IRoom _room;
    private EndGuessingGameCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _context.Room.Returns(_room);
        _command = new EndGuessingGameCommand();
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoGame_WhenNoGuessingGameIsRunning()
    {
        _room.Game.Returns((IGame)null);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("end_guessing_game_no_game");
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoGame_WhenDifferentGameIsRunning()
    {
        var otherGame = Substitute.For<IGame>();
        _room.Game.Returns(otherGame);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("end_guessing_game_no_game");
    }

    [Test]
    public void Test_RunAsync_ShouldStopGameAndReplySuccess_WhenGuessingGameIsRunning()
    {
        var guessingGame = Substitute.For<IGuessingGame, IGame>();
        _room.Game.Returns((IGame)guessingGame);

        _command.RunAsync(_context);

        guessingGame.Received(1).StopGame();
        _context.Received(1).ReplyLocalizedMessage("end_guessing_game_success");
    }

    [Test]
    public void Test_RunAsync_ShouldNotReplyNoGame_WhenGuessingGameIsStopped()
    {
        var guessingGame = Substitute.For<IGuessingGame, IGame>();
        _room.Game.Returns((IGame)guessingGame);

        _command.RunAsync(_context);

        _context.DidNotReceive().ReplyLocalizedMessage("end_guessing_game_no_game");
    }
}
