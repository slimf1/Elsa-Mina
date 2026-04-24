using ElsaMina.Commands.Tournaments.Betting;
using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Templates;
using NSubstitute;
using NUnit.Framework;

namespace ElsaMina.UnitTests.Commands.Tournaments.Betting;

[TestFixture]
public class TournamentBettingServiceTest
{
    private IBot _bot;
    private ITemplatesManager _templatesManager;
    private IConfiguration _configuration;
    private TournamentBettingService _service;

    [SetUp]
    public void SetUp()
    {
        _bot = Substitute.For<IBot>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _configuration = Substitute.For<IConfiguration>();
        _configuration.Name.Returns("Elsa-Mina");
        _configuration.Trigger.Returns("-");
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>())
            .Returns("<html/>");
        _service = new TournamentBettingService(_bot, _templatesManager, _configuration);
    }

    // ── AnnounceBetsAsync ──────────────────────────────────────────────────

    [Test]
    public async Task Test_AnnounceBetsAsync_ShouldSendHtmlToRoom()
    {
        await _service.AnnounceBetsAsync(["PlayerA", "PlayerB"], "room1");

        _bot.Received(1).Say("room1", Arg.Is<string>(s => s.StartsWith("/addhtmlbox")));
    }

    [Test]
    public async Task Test_AnnounceBetsAsync_ShouldOpenBettingSession_SoPlaceBetSucceeds()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");

        var result = await _service.PlaceBetAsync("bettor1", "playera", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.Success));
    }

    [Test]
    public async Task Test_AnnounceBetsAsync_ShouldNormalizePlayers_ToLowerAlphaNum()
    {
        await _service.AnnounceBetsAsync(["Player A!"], "room1");

        var result = await _service.PlaceBetAsync("bettor1", "playera", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.Success));
    }

    // ── PlaceBetAsync ──────────────────────────────────────────────────────

    [Test]
    public async Task Test_PlaceBetAsync_ShouldReturnNoBettingSession_WhenNoActiveSession()
    {
        var result = await _service.PlaceBetAsync("bettor1", "playerA", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.NoBettingSession));
    }

    [Test]
    public async Task Test_PlaceBetAsync_ShouldReturnInvalidPlayer_WhenPlayerNotInTournament()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");

        var result = await _service.PlaceBetAsync("bettor1", "nobody", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.InvalidPlayer));
    }

    [Test]
    public async Task Test_PlaceBetAsync_ShouldReturnSuccess_WhenBetIsValid()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");

        var result = await _service.PlaceBetAsync("bettor1", "playera", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.Success));
    }

    [Test]
    public async Task Test_PlaceBetAsync_ShouldReturnAlreadyBetOnPlayer_WhenSameBettorTargetsSamePlayerTwice()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");

        var result = await _service.PlaceBetAsync("bettor1", "playera", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.AlreadyBetOnPlayer));
    }

    [Test]
    public async Task Test_PlaceBetAsync_ShouldReturnSuccess_WhenSameBettorBetsOnDifferentPlayers()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");

        var result = await _service.PlaceBetAsync("bettor1", "playerb", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.Success));
    }

    [Test]
    public async Task Test_PlaceBetAsync_ShouldReturnSuccess_WhenDifferentBettorsTargetSamePlayer()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");

        var result = await _service.PlaceBetAsync("bettor2", "playera", "room1");

        Assert.That(result, Is.EqualTo(BetPlacementError.Success));
    }

    [Test]
    public async Task Test_PlaceBetAsync_ShouldBeIsolatedByRoom()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");

        var result = await _service.PlaceBetAsync("bettor1", "playera", "room2");

        Assert.That(result, Is.EqualTo(BetPlacementError.NoBettingSession));
    }

    // ── CancelBetAsync ─────────────────────────────────────────────────────

    [Test]
    public async Task Test_CancelBetAsync_ShouldReturnZero_WhenNoActiveSession()
    {
        var count = await _service.CancelBetAsync("bettor1", "room1");

        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task Test_CancelBetAsync_ShouldReturnZero_WhenNoBetExists()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");

        var count = await _service.CancelBetAsync("bettor1", "room1");

        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task Test_CancelBetAsync_ShouldReturnOne_AndRemoveBet_WhenNoPlayerSpecified()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");

        var count = await _service.CancelBetAsync("bettor1", "room1");

        Assert.That(count, Is.EqualTo(1));
        // Placing the same bet again should succeed (slot is free)
        var result = await _service.PlaceBetAsync("bettor1", "playera", "room1");
        Assert.That(result, Is.EqualTo(BetPlacementError.Success));
    }

    [Test]
    public async Task Test_CancelBetAsync_ShouldCancelAllBets_WhenBettorHasMultipleAndNoPlayerSpecified()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");
        await _service.PlaceBetAsync("bettor1", "playerb", "room1");

        var count = await _service.CancelBetAsync("bettor1", "room1");

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public async Task Test_CancelBetAsync_ShouldCancelOnlySpecifiedPlayer_WhenPlayerProvided()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");
        await _service.PlaceBetAsync("bettor1", "playerb", "room1");

        var count = await _service.CancelBetAsync("bettor1", "room1", "playera");

        Assert.That(count, Is.EqualTo(1));
        // playerB bet must still be there — placing again on playerB must be rejected
        var result = await _service.PlaceBetAsync("bettor1", "playerb", "room1");
        Assert.That(result, Is.EqualTo(BetPlacementError.AlreadyBetOnPlayer));
    }

    [Test]
    public async Task Test_CancelBetAsync_ShouldNotAffectOtherBettors()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");
        await _service.PlaceBetAsync("bettor2", "playera", "room1");

        await _service.CancelBetAsync("bettor1", "room1");

        // bettor2's bet must survive
        var result = await _service.PlaceBetAsync("bettor2", "playera", "room1");
        Assert.That(result, Is.EqualTo(BetPlacementError.AlreadyBetOnPlayer));
    }

    // ── ResolveBetsAsync ───────────────────────────────────────────────────

    [Test]
    public async Task Test_ResolveBetsAsync_ShouldDoNothing_WhenNoBetsExist()
    {
        await _service.ResolveBetsAsync("playerA", "room1");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_ResolveBetsAsync_ShouldAnnounceCorrectGuessers_WhenSomeGuessedRight()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");
        await _service.PlaceBetAsync("bettor2", "playerb", "room1");

        await _service.ResolveBetsAsync("playera", "room1");

        _bot.Received(1).Say("room1", Arg.Is<string>(s => s.Contains("bettor1") && !s.Contains("bettor2")));
    }

    [Test]
    public async Task Test_ResolveBetsAsync_ShouldAnnounceNobodyGuessedCorrectly_WhenNoCorrectBets()
    {
        await _service.AnnounceBetsAsync(["playerA", "playerB"], "room1");
        await _service.PlaceBetAsync("bettor1", "playerb", "room1");

        await _service.ResolveBetsAsync("playera", "room1");

        _bot.Received(1).Say("room1", Arg.Is<string>(s => s.Contains("Nobody")));
    }

    [Test]
    public async Task Test_ResolveBetsAsync_ShouldCleanUp_SoFurtherBetsAreRejected()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");

        await _service.ResolveBetsAsync("playera", "room1");

        var result = await _service.PlaceBetAsync("bettor2", "playera", "room1");
        Assert.That(result, Is.EqualTo(BetPlacementError.NoBettingSession));
    }

    // ── ReturnBetsAsync ────────────────────────────────────────────────────

    [Test]
    public async Task Test_ReturnBetsAsync_ShouldCleanUp_SoFurtherBetsAreRejected()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        await _service.PlaceBetAsync("bettor1", "playera", "room1");

        await _service.ReturnBetsAsync("room1");

        var result = await _service.PlaceBetAsync("bettor2", "playera", "room1");
        Assert.That(result, Is.EqualTo(BetPlacementError.NoBettingSession));
    }

    [Test]
    public async Task Test_ReturnBetsAsync_ShouldNotSayAnything()
    {
        await _service.AnnounceBetsAsync(["playerA"], "room1");
        _bot.ClearReceivedCalls();

        await _service.ReturnBetsAsync("room1");

        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }
}
