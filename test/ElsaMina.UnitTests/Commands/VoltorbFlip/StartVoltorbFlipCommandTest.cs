using ElsaMina.Commands.VoltorbFlip;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.VoltorbFlip;

public class StartVoltorbFlipCommandTest
{
    private IDependencyContainerService _dependencyContainerService;
    private IConfiguration _configuration;
    private ITemplatesManager _templatesManager;
    private StartVoltorbFlipCommand _command;
    private IContext _context;
    private IRoom _room;
    private VoltorbFlipGame _game;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _configuration = Substitute.For<IConfiguration>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _command = new StartVoltorbFlipCommand(_dependencyContainerService);

        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _context.Room.Returns(_room);
        _context.RoomId.Returns("test-room");

        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>())
            .Returns(Task.FromResult(string.Empty));

        var randomService = Substitute.For<IRandomService>();
        randomService.RandomElement(Arg.Any<IList<(int Twos, int Threes, int Voltorbs)>>())
            .Returns((1, 0, 1));
        _game = new VoltorbFlipGame(randomService, _templatesManager, _configuration);
        _dependencyContainerService.Resolve<VoltorbFlipGame>().Returns(_game);
    }

    [Test]
    public void Test_RequiredRank_ShouldReturnVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public async Task Test_RunAsync_ShouldCreateGame_WhenNoGameExists()
    {
        // Arrange
        _room.Game = null;

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dependencyContainerService.Received(1).Resolve<VoltorbFlipGame>();
        Assert.That(_room.Game, Is.SameAs(_game));
    }

    [Test]
    public async Task Test_RunAsync_ShouldShowAnnounce_WhenNoGameExists()
    {
        // Arrange
        _room.Game = null;

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).SendUpdatableHtml(Arg.Any<string>(), Arg.Any<string>(), false);
        Assert.That(_game.IsRoundActive, Is.False);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyAlreadyRunning_WhenOtherGameExists()
    {
        // Arrange
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("vf_game_already_running");
        _dependencyContainerService.DidNotReceive().Resolve<VoltorbFlipGame>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWaiting_WhenVoltorbFlipGameIsNotYetStarted()
    {
        // Arrange
        var existingGame = Substitute.For<IVoltorbFlipGame>();
        existingGame.IsStarted.Returns(false);
        _room.Game.Returns(existingGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("vf_game_waiting");
        _dependencyContainerService.DidNotReceive().Resolve<VoltorbFlipGame>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyRoundActive_WhenVoltorbFlipGameHasActiveRound()
    {
        // Arrange
        var existingGame = Substitute.For<IVoltorbFlipGame>();
        existingGame.IsStarted.Returns(true);
        existingGame.IsRoundActive.Returns(true);
        _room.Game.Returns(existingGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("vf_game_round_active");
        _dependencyContainerService.DidNotReceive().Resolve<VoltorbFlipGame>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldStartNewRound_WhenVoltorbFlipGameExistsWithNoActiveRound()
    {
        // Arrange
        var existingGame = Substitute.For<IVoltorbFlipGame>();
        existingGame.IsStarted.Returns(true);
        existingGame.IsRoundActive.Returns(false);
        _room.Game.Returns(existingGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await existingGame.Received(1).StartNewRound();
        _dependencyContainerService.DidNotReceive().Resolve<VoltorbFlipGame>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateContext_WhenVoltorbFlipGameExistsWithNoActiveRound()
    {
        // Arrange
        var existingGame = Substitute.For<IVoltorbFlipGame>();
        existingGame.IsStarted.Returns(true);
        existingGame.IsRoundActive.Returns(false);
        _room.Game.Returns(existingGame);

        // Act
        await _command.RunAsync(_context);

        // Assert
        Assert.That(existingGame.Context, Is.SameAs(_context));
    }
}
