using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using NSubstitute;

namespace ElsaMina.Test.Commands.ConnectFour;

public class CreateConnectFourCommandTest
{
    private IRoomsManager _roomsManager;
    private IDependencyContainerService _dependencyContainerService;
    private IConfigurationManager _configurationManager;
    private CreateConnectFourCommand _command;
    private IContext _context;
    private IRoom _room;
    private ITemplatesManager _templatesManager;
    private ConnectFourGame _game;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _command = new CreateConnectFourCommand(_roomsManager, _dependencyContainerService);

        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _game = new ConnectFourGame(Substitute.For<IRandomService>(), _templatesManager, _configurationManager);

        _context.RoomId.Returns("room-id");
        _roomsManager.GetRoom("room-id").Returns(_room);
        _dependencyContainerService.Resolve<ConnectFourGame>().Returns(_game);
    }

    [Test]
    public async Task Test_Run_ShouldAnnounceGameStart_WhenNoGameAlreadyExists()
    {
        // Arrange
        _room.Game = null;
        _configurationManager.Configuration.Trigger.Returns("!");

        // Act
        await _command.Run(_context);

        // Assert
        _dependencyContainerService.Received(1).Resolve<ConnectFourGame>();
        await _templatesManager.GetTemplate("ConnectFour/ConnectFourGamePanel", Arg.Any<object>());
        Assert.That(_room.Game, Is.SameAs(_game));
    }

    [Test]
    public async Task Test_Run_ShouldNotStartGame_WhenGameAlreadyExists()
    {
        // Act
        await _command.Run(_context);

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