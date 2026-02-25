using ElsaMina.Commands.GuessingGame;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.GuessingGame;

public class GuessingGameHandlerTest
{
    private IContextFactory _contextFactory;
    private GuessingGameHandler _handler;
    private IContext _context;
    private IRoom _room;
    private IUser _sender;

    [SetUp]
    public void SetUp()
    {
        _contextFactory = Substitute.For<IContextFactory>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();
        _sender = Substitute.For<IUser>();

        _sender.Name.Returns("Player1");
        _context.Sender.Returns(_sender);
        _context.Message.Returns("pikachu");
        _context.Room.Returns(_room);

        _handler = new GuessingGameHandler(_contextFactory);
    }

    [Test]
    public async Task Test_HandleMessageAsync_ShouldNotCallOnAnswer_WhenRoomHasNoGame()
    {
        // Arrange
        _room.Game.ReturnsNull();

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _handler.HandleMessageAsync(_context));
    }

    [Test]
    public async Task Test_HandleMessageAsync_ShouldNotCallOnAnswer_WhenGameIsNotGuessingGame()
    {
        // Arrange
        _room.Game.Returns(Substitute.For<IGame>());

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _handler.HandleMessageAsync(_context));
    }

    [Test]
    public async Task Test_HandleMessageAsync_ShouldCallOnAnswer_WhenGameIsGuessingGame()
    {
        // Arrange
        var guessingGame = Substitute.For<IGame, IGuessingGame>();
        _room.Game.Returns(guessingGame);

        // Act
        await _handler.HandleMessageAsync(_context);

        // Assert
        ((IGuessingGame)guessingGame).Received(1).OnAnswer("Player1", "pikachu");
    }
}
