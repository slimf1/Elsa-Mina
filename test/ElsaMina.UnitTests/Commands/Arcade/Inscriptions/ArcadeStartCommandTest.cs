using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Inscriptions;

[TestFixture]
public class ArcadeStartCommandTest
{
    private IArcadeInscriptionsManager _inscriptionsManager;
    private IContext _context;
    private ArcadeStartCommand _command;

    [SetUp]
    public void SetUp()
    {
        _inscriptionsManager = Substitute.For<IArcadeInscriptionsManager>();
        _context = Substitute.For<IContext>();
        _context.RoomId.Returns("testroom");
        _command = new ArcadeStartCommand(_inscriptionsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_start_help"));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyAlreadyActive_WhenInscriptionsAreAlreadyActive()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_start_already_active");
        _inscriptionsManager.DidNotReceive().InitInscriptions(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void Test_RunAsync_ShouldUseDefaultTitle_WhenTargetIsEmpty()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns(string.Empty);

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).InitInscriptions("testroom", "Tournoi Arcade");
        _context.Received(1).ReplyLocalizedMessage("arcade_start_no_timer", "Tournoi Arcade");
    }

    [Test]
    public void Test_RunAsync_ShouldUseDefaultTitle_WhenTargetIsWhitespace()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns("   ");

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).InitInscriptions("testroom", "Tournoi Arcade");
        _context.Received(1).ReplyLocalizedMessage("arcade_start_no_timer", "Tournoi Arcade");
    }

    [Test]
    public void Test_RunAsync_ShouldUseCustomTitle_WhenTitleIsProvided()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns("My Custom Tournament");

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).InitInscriptions("testroom", "My Custom Tournament");
        _context.Received(1).ReplyLocalizedMessage("arcade_start_no_timer", "My Custom Tournament");
    }

    [Test]
    public void Test_RunAsync_ShouldStartTimerAndReply_WhenTitleAndValidTimerAreProvided()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns("My Tournament, 5");

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).InitInscriptions("testroom", "My Tournament");
        _inscriptionsManager.Received(1).StartTimer("testroom", 5);
        _context.Received(1).ReplyLocalizedMessage("arcade_start_with_timer", "My Tournament", 5);
    }

    [Test]
    public void Test_RunAsync_ShouldReplyParseError_WhenTimerIsNotAnInteger()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns("My Tournament, abc");

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_timer_parse_error");
        _inscriptionsManager.DidNotReceive().InitInscriptions(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyPositiveInteger_WhenTimerIsZero()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns("My Tournament, 0");

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_timer_positive_integer");
        _inscriptionsManager.DidNotReceive().InitInscriptions(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyPositiveInteger_WhenTimerIsNegative()
    {
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);
        _context.Target.Returns("My Tournament, -3");

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_timer_positive_integer");
        _inscriptionsManager.DidNotReceive().InitInscriptions(Arg.Any<string>(), Arg.Any<string>());
    }
}
