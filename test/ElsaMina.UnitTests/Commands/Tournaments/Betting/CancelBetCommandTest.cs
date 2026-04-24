using ElsaMina.Commands.Tournaments.Betting;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NUnit.Framework;

namespace ElsaMina.UnitTests.Commands.Tournaments.Betting;

[TestFixture]
public class CancelBetCommandTest
{
    private IContext _context;
    private IUser _sender;
    private ITournamentBettingService _bettingService;
    private CancelBetCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _sender = Substitute.For<IUser>();
        _sender.UserId.Returns("bettor1");
        _context.Sender.Returns(_sender);
        _bettingService = Substitute.For<ITournamentBettingService>();
        _command = new CancelBetCommand(_bettingService);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelp_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("cancelbet_help");
        await _bettingService.DidNotReceive()
            .CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldCancelAllBets_WhenOnlyRoomIdProvided()
    {
        _context.Target.Returns("room1");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1);

        await _command.RunAsync(_context);

        await _bettingService.Received(1).CancelBetAsync("bettor1", "room1", null, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldCancelSpecificBet_WhenRoomAndPlayerProvided()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1);

        await _command.RunAsync(_context);

        await _bettingService.Received(1).CancelBetAsync("bettor1", "room1", "playera", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyAllSuccess_WhenAllBetsCancelled()
    {
        _context.Target.Returns("room1");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(2);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("cancelbet_all_success", "room1");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplySuccess_WhenSpecificBetCancelled()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("cancelbet_success", "playera");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenNoBetToCancel_WithNoPlayer()
    {
        _context.Target.Returns("room1");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(0);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("cancelbet_not_found", "room1");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenNoBetToCancel_WithPlayer()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(0);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("cancelbet_not_found", "playera");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizeRoomAndPlayer_ToLowerAlphaNum()
    {
        _context.Target.Returns("Room1 Player A!");
        _bettingService.CancelBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(1);

        await _command.RunAsync(_context);

        await _bettingService.Received(1).CancelBetAsync("bettor1", "room1", "playera", Arg.Any<CancellationToken>());
    }
}
