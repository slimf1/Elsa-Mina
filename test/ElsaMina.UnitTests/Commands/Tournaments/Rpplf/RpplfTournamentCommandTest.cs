using ElsaMina.Core.Contexts;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Tournaments.Rpplf;

[TestFixture]
public class RpplfTournamentCommandTest
{
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendCreateAutostartNameTeamsAndRules()
    {
        // Arrange
        var command = new TestRpplfTournamentCommand(
            format: "gen9ubers",
            tourName: "[Gen 9] RPPLF",
            teamsName: "gen9rpplf",
            tourRules: "Item Clause=1");

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply("/tour create gen9ubers, elim");
        _context.Received(1).Reply("/tour autostart 10");
        _context.Received(1).Reply("/tour name [Gen 9] RPPLF");
        _context.Received(1).Reply("-teams gen9rpplf");
        _context.Received(1).Reply("/tour rules Item Clause=1");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendAutoDq_WhenAutoDqIsSet()
    {
        // Arrange
        var command = new TestRpplfTournamentCommand(autoDq: 5);

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.Received(1).Reply("/tour autodq 5");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotSendAutoDq_WhenAutoDqIsNull()
    {
        // Arrange
        var command = new TestRpplfTournamentCommand(autoDq: null);

        // Act
        await command.RunAsync(_context);

        // Assert
        _context.DidNotReceive().Reply(Arg.Is<string>(s => s.StartsWith("/tour autodq")));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendCommandsInOrder()
    {
        // Arrange
        var command = new TestRpplfTournamentCommand(autoDq: 5);
        var replies = new List<string>();
        _context.When(context => context.Reply(Arg.Any<string>()))
            .Do(call => replies.Add(call.Arg<string>()));

        // Act
        await command.RunAsync(_context);

        // Assert
        Assert.That(replies[0], Does.StartWith("/tour create"));
        Assert.That(replies[1], Is.EqualTo("/tour autostart 10"));
        Assert.That(replies[2], Is.EqualTo("/tour autodq 5"));
        Assert.That(replies[3], Does.StartWith("/tour name"));
        Assert.That(replies[4], Does.StartWith("-teams"));
        Assert.That(replies[5], Does.StartWith("/tour rules"));
    }
}
