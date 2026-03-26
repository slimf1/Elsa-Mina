using ElsaMina.Commands.Misc;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Misc;

[TestFixture]
public class TimerCommandTest
{
    private IContext _context;
    private TimerCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _command = new TimerCommand();
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeTimerCommandHelp()
    {
        Assert.That(_command.HelpMessageKey, Is.EqualTo("timer_command_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenTargetCannotBeParsedAsTimeSpan()
    {
        _context.Target.Returns("notaduration");

        await _command.RunAsync(_context);

        _context.Received(1).Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
        _context.DidNotReceive().ReplyRankAwareLocalizedMessage("timer_command_started", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);

        await _command.RunAsync(_context);

        _context.Received(1).Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
        _context.DidNotReceive().ReplyRankAwareLocalizedMessage("timer_command_started", Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotifyStartAndSuccess_WhenDurationIsValid()
    {
        var user = Substitute.For<IUser>();
        user.Name.Returns("TestUser");
        _context.Sender.Returns(user);
        _context.Target.Returns("1ms");

        await _command.RunAsync(_context);

        _context.Received(1).ReplyRankAwareLocalizedMessage("timer_command_started");
        _context.Received(1).ReplyRankAwareLocalizedMessage("timer_command_success", "TestUser");
    }
}
