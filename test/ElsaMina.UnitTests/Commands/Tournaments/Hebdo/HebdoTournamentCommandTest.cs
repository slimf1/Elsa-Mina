using ElsaMina.Core.Contexts;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Tournaments.Hebdo;

public class HebdoTournamentCommandTest
{
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWrongRoom_WhenNotInArcadeRoom()
    {
        // Arrange
        _context.RoomId.Returns("someroom");
        var command = new TestHebdoTournamentCommand();

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("hebdo_wrong_room");
        _context.DidNotReceive().Reply(Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendTourCommands_WhenInArcadeRoom()
    {
        // Arrange
        _context.RoomId.Returns("arcade");
        var command = new TestHebdoTournamentCommand(format: "ou", autostart: 8, tourName: "Tournoi Hebdo");

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply("/tour create ou, elim");
        _context.Received(1).Reply("/tour autostart 8");
        _context.Received(1).Reply("/tour name Tournoi Hebdo");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendWallMessage_WhenWallMessageIsSet()
    {
        // Arrange
        _context.RoomId.Returns("arcade");
        var command = new TestHebdoTournamentCommand(wallMessage: "Hebdo OM FR en AAA !");

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply("/wall Hebdo OM FR en AAA !");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotSendWallMessage_WhenWallMessageIsNull()
    {
        // Arrange
        _context.RoomId.Returns("arcade");
        var command = new TestHebdoTournamentCommand(wallMessage: null);

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.DidNotReceive().Reply(Arg.Is<string>(s => s.StartsWith("/wall")));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRoomEvents_WhenRoomEventsNameIsSet()
    {
        // Arrange
        _context.RoomId.Returns("arcade");
        var command = new TestHebdoTournamentCommand(roomEventsName: "RU Classic");

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply("/roomevents start RU Classic");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotSendRoomEvents_WhenRoomEventsNameIsNull()
    {
        // Arrange
        _context.RoomId.Returns("arcade");
        var command = new TestHebdoTournamentCommand(roomEventsName: null);

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.DidNotReceive().Reply(Arg.Is<string>(s => s.StartsWith("/roomevents")));
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotReplyWrongRoom_WhenInArcadeRoom()
    {
        // Arrange
        _context.RoomId.Returns("arcade");
        var command = new TestHebdoTournamentCommand();

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.DidNotReceive().ReplyLocalizedMessage(Arg.Any<string>());
    }
}
