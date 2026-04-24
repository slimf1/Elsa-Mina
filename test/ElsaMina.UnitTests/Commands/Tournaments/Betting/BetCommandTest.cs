using ElsaMina.Commands.Tournaments.Betting;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;
using NUnit.Framework;

namespace ElsaMina.UnitTests.Commands.Tournaments.Betting;

[TestFixture]
public class BetCommandTest
{
    private IContext _context;
    private IUser _sender;
    private ITournamentBettingService _bettingService;
    private BetCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _sender = Substitute.For<IUser>();
        _sender.UserId.Returns("bettor1");
        _context.Sender.Returns(_sender);
        _bettingService = Substitute.For<ITournamentBettingService>();
        _command = new BetCommand(_bettingService);
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
    public async Task Test_RunAsync_ShouldReplyHelp_WhenTargetHasNoSpace()
    {
        _context.Target.Returns("room1");

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("bet_help");
        await _bettingService.DidNotReceive()
            .PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelp_WhenTargetIsEmpty()
    {
        _context.Target.Returns(string.Empty);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("bet_help");
    }

    [Test]
    public async Task Test_RunAsync_ShouldCallPlaceBet_WithParsedRoomAndPlayer()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(BetPlacementError.Success);

        await _command.RunAsync(_context);

        await _bettingService.Received(1).PlaceBetAsync("bettor1", "playera", "room1", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplySuccess_WhenBetPlaced()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(BetPlacementError.Success);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("bet_success", "playera", "room1");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNoSession_WhenNoBettingSession()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(BetPlacementError.NoBettingSession);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("bet_no_session", "room1");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyInvalidPlayer_WhenPlayerNotInTournament()
    {
        _context.Target.Returns("room1 nobody");
        _bettingService.PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(BetPlacementError.InvalidPlayer);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("bet_invalid_player", "nobody");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyAlreadyBetOnPlayer_WhenDuplicateBet()
    {
        _context.Target.Returns("room1 playerA");
        _bettingService.PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(BetPlacementError.AlreadyBetOnPlayer);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("bet_already_on_player", "playera");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNormalizePlayer_ToLowerAlphaNum()
    {
        _context.Target.Returns("room1 Player A!");
        _bettingService.PlaceBetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(BetPlacementError.Success);

        await _command.RunAsync(_context);

        await _bettingService.Received(1).PlaceBetAsync("bettor1", "playera", "room1", Arg.Any<CancellationToken>());
    }
}
