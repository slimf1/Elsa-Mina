using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.ConnectFour;

public class CreateConnectFourCommandTest
{
    private IDependencyContainerService _dependencyContainerService;
    private IConfiguration _configuration;
    private CreateConnectFourCommand _command;
    private IContext _context;
    private IRoom _room;
    private ITemplatesManager _templatesManager;
    private IBot _bot;
    private ConnectFourGame _game;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _configuration = Substitute.For<IConfiguration>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _bot = Substitute.For<IBot>();
        _command = new CreateConnectFourCommand(_dependencyContainerService);

        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _room.RoomId.Returns("room-id");
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>())
            .Returns(Task.FromResult(string.Empty));
        _game = new ConnectFourGame(Substitute.For<IRandomService>(), _templatesManager,
            _configuration, _bot, ConnectFourConstants.TIMEOUT_DELAY);

        _context.RoomId.Returns("room-id");
        _context.Room.Returns(_room);
        _dependencyContainerService.Resolve<ConnectFourGame>().Returns(_game);
    }

    [Test]
    public async Task Test_RunAsync_ShouldAnnounceGameStart_WhenNoGameAlreadyExists()
    {
        // Arrange
        _room.Game = null;
        _configuration.Trigger.Returns("!");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _dependencyContainerService.Received(1).Resolve<ConnectFourGame>();
        await _templatesManager.GetTemplateAsync("ConnectFour/ConnectFourGamePanel", Arg.Any<object>());
        Assert.That(_room.Game, Is.SameAs(_game));
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotStartGame_WhenGameAlreadyExists()
    {
        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("c4_game_start_already_exist");
        _dependencyContainerService.DidNotReceive().Resolve<ConnectFourGame>();
    }

    [Test]
    public void Test_RequiredRank_ShouldReturnCorrectRank()
    {
        // Act
        var rank = _command.RequiredRank;

        // Assert
        Assert.That(rank, Is.EqualTo(Rank.Voiced));
    }
}
