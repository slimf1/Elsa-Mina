using ElsaMina.Commands.GuessingGame;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.UnitTests.Commands.ConnectFour;

public class GuessingGameCommandTest
{
    private GuessingGameCommand _command;
    private IDependencyContainerService _dependencyContainerService;
    private IContext _context;
    private IRoom _room;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _context = Substitute.For<IContext>();
        _room = Substitute.For<IRoom>();

        _command = new GuessingGameCommand(_dependencyContainerService);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplySpecifyMessage_WhenTurnsCountIsInvalid()
    {
        // Arrange
        _context.Target.Returns("invalid");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("guessing_game_specify");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyInvalidTurnsMessage_WhenTurnsCountIsOutOfRange()
    {
        // Arrange
        _context.Target.Returns("25");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("guessing_game_invalid_number_turns", 20);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyOngoingGameMessage_WhenGameIsAlreadyRunning()
    {
        // Arrange
        _context.Target.Returns("10");
        _context.Room.Returns(_room);
        _room.Game.Returns(Substitute.For<IGame>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("guessing_game_currently_ongoing");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyInvalidCommand_WhenCommandIsUnknown()
    {
        // Arrange
        _context.Target.Returns("5");
        _context.Command.Returns("unknown");
        _room.Game.ReturnsNull();
        _context.Room.Returns(_room);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("guessing_game_invalid_command");
    }
}