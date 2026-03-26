using ElsaMina.Commands.PokeRace;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.PokeRace;

[TestFixture]
public class StartPokeRaceCommandTest
{
    private IDependencyContainerService _dependencyContainerService;
    private IContext _context;
    private IRoom _room;
    private StartPokeRaceCommand _command;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _context.Room.Returns(_room);
        _command = new StartPokeRaceCommand(_dependencyContainerService);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyAlreadyRunning_WhenPokeRaceGameIsRunning()
    {
        var existingGame = Substitute.For<IPokeRaceGame>();
        _room.Game.Returns(existingGame);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("pokerace_already_running");
        _dependencyContainerService.DidNotReceive().Resolve<PokeRaceGame>();
    }

    [Test]
    public void Test_RunAsync_ShouldReplyOtherGameRunning_WhenDifferentGameIsRunning()
    {
        var otherGame = Substitute.For<IGame>();
        _room.Game.Returns(otherGame);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("pokerace_other_game_running");
        _dependencyContainerService.DidNotReceive().Resolve<PokeRaceGame>();
    }

    [Test]
    public void Test_RunAsync_ShouldStartGame_WhenNoGameIsRunning()
    {
        _room.Game.Returns((IGame)null);
        var randomService = Substitute.For<IRandomService>();
        var game = new PokeRaceGame(randomService, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        _dependencyContainerService.Resolve<PokeRaceGame>().Returns(game);

        _command.RunAsync(_context);

        _context.DidNotReceive().ReplyLocalizedMessage("pokerace_already_running");
        _context.DidNotReceive().ReplyLocalizedMessage("pokerace_other_game_running");
        _dependencyContainerService.Received(1).Resolve<PokeRaceGame>();
    }

    [Test]
    public void Test_RunAsync_ShouldAssignContextAndRoomGame_WhenStartingGame()
    {
        _room.Game.Returns((IGame)null);
        var randomService = Substitute.For<IRandomService>();
        var game = new PokeRaceGame(randomService, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        _dependencyContainerService.Resolve<PokeRaceGame>().Returns(game);

        _command.RunAsync(_context);

        Assert.That(game.Context, Is.SameAs(_context));
        _room.Received(1).Game = game;
    }
}
