using ElsaMina.Commands.ConnectFour;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using NSubstitute;

namespace ElsaMina.Test.Commands.ConnectFour;

public class ConnectFourGameTest
{
    public class ConnectFourTests // Todo : timeout and tie
    {
        private ConnectFourGame _game;
        private IRandomService _mockRandomService;
        private ITemplatesManager _mockTemplatesManager;
        private IConfiguration _configuration;
        private IDependencyContainerService _dependencyContainerService;
        private IBot _bot;
        private IContext _context;
        private IUser _mockUser1;
        private IUser _mockUser2;

        [SetUp]
        public void SetUp()
        {
            _mockRandomService = Substitute.For<IRandomService>();
            _mockTemplatesManager = Substitute.For<ITemplatesManager>();
            _configuration = Substitute.For<IConfiguration>();
            _context = Substitute.For<IContext>();
            _bot = Substitute.For<IBot>();
            _dependencyContainerService = Substitute.For<IDependencyContainerService>();

            DependencyContainerService.Current = _dependencyContainerService;

            _configuration.Name.Returns("Bot");
            _configuration.Trigger.Returns("!");
            _configuration.DefaultLocaleCode.Returns("fr-FR");

            _game = new ConnectFourGame(_mockRandomService, _mockTemplatesManager, _configuration, _bot);
            _game.Context = _context;

            _mockUser1 = Substitute.For<IUser>();
            _mockUser2 = Substitute.For<IUser>();
            _mockUser1.Name.Returns("Player1");
            _mockUser2.Name.Returns("Player2");
        }

        [TearDown]
        public void TearDown()
        {
            DependencyContainerService.Current = null;
        }

        [Test]
        public async Task JoinGame_ShouldAddPlayers_WhenGameIsNotStarted()
        {
            await _game.JoinGame(_mockUser1);
            await _game.JoinGame(_mockUser2);

            Assert.Multiple(() =>
            {
                Assert.That(_game.Players, Has.Member(_mockUser1));
                Assert.That(_game.Players, Has.Member(_mockUser2));
                Assert.That(_game.TurnCount, Is.EqualTo(1));
                Assert.That(_game.Players, Has.Count.EqualTo(2));
            });
        }

        [Test]
        public async Task JoinGame_ShouldNotAddPlayers_WhenGameIsStarted()
        {
            await _game.JoinGame(_mockUser1);
            await _game.JoinGame(_mockUser2);

            await _game.Play(_mockUser1, "1");

            var mockUser3 = Substitute.For<IUser>();
            await _game.JoinGame(mockUser3);

            Assert.Multiple(() =>
            {
                Assert.That(_game.Players, Does.Not.Contain(mockUser3));
                Assert.That(_game.Players, Has.Count.EqualTo(2));
                Assert.That(_game.TurnCount, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task Play_ShouldMakeMove_WhenInputIsValid()
        {
            await _game.JoinGame(_mockUser1);
            await _game.JoinGame(_mockUser2);
            await _game.Play(_mockUser1, "100");
            Assert.That(_game.TurnCount, Is.EqualTo(1));
        }

        [Test]
        public async Task OnTimeout_ShouldDisqualifyPlayer_WhenTheyTimeout()
        {
            await _game.JoinGame(_mockUser1);
            await _game.JoinGame(_mockUser2);

            await _game.OnTimeout();
            Assert.That(_game.Players, Does.Not.Contain(_mockUser1));
        }

        [Test]
        public async Task Game_ShouldDeclareWinner_WhenPlayerWins()
        {
            await _game.JoinGame(_mockUser1);
            await _game.JoinGame(_mockUser2);
            await _game.Play(_mockUser1, "1");
            await _game.Play(_mockUser2, "2");
            await _game.Play(_mockUser1, "1");
            await _game.Play(_mockUser2, "2");
            await _game.Play(_mockUser1, "1");
            await _game.Play(_mockUser2, "2");
            await _game.Play(_mockUser1, "1");

            _context.Received(1).ReplyLocalizedMessage("c4_game_win_message", _mockUser1.Name);
        }
    }
}