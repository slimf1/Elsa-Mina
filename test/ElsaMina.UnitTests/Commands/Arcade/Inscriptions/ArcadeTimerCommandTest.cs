using ElsaMina.Commands.Arcade.Inscriptions;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade.Inscriptions;

[TestFixture]
public class ArcadeTimerCommandTest
{
    private IArcadeInscriptionsManager _inscriptionsManager;
    private IContext _context;
    private ArcadeTimerCommand _command;

    [SetUp]
    public void SetUp()
    {
        _inscriptionsManager = Substitute.For<IArcadeInscriptionsManager>();
        _context = Substitute.For<IContext>();
        _context.RoomId.Returns("testroom");
        _command = new ArcadeTimerCommand(_inscriptionsManager);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("arcade_timer_help"));
    }

    [Test]
    public void Test_RunAsync_ShouldReplyHelp_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);

        _command.RunAsync(_context);

        _context.Received(1).Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyNoActiveInscriptions_WhenNoneActive()
    {
        _context.Target.Returns("5");
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(false);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_no_active_inscriptions");
        _inscriptionsManager.DidNotReceive().StartTimer(Arg.Any<string>(), Arg.Any<int>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyParseError_WhenTargetIsNotAnInteger()
    {
        _context.Target.Returns("notanumber");
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_timer_parse_error");
        _inscriptionsManager.DidNotReceive().StartTimer(Arg.Any<string>(), Arg.Any<int>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyPositiveInteger_WhenTimerIsZero()
    {
        _context.Target.Returns("0");
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_timer_positive_integer");
        _inscriptionsManager.DidNotReceive().StartTimer(Arg.Any<string>(), Arg.Any<int>());
    }

    [Test]
    public void Test_RunAsync_ShouldReplyPositiveInteger_WhenTimerIsNegative()
    {
        _context.Target.Returns("-1");
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);

        _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("arcade_timer_positive_integer");
        _inscriptionsManager.DidNotReceive().StartTimer(Arg.Any<string>(), Arg.Any<int>());
    }

    [Test]
    public void Test_RunAsync_ShouldStartTimerAndReplySuccess_WhenValidMinutesProvided()
    {
        _context.Target.Returns("10");
        _inscriptionsManager.HasActiveInscriptions("testroom").Returns(true);

        _command.RunAsync(_context);

        _inscriptionsManager.Received(1).StartTimer("testroom", 10);
        _context.Received(1).ReplyLocalizedMessage("arcade_timer_success", 10);
    }
}
