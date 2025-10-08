using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Templates;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Showdown.Ranking;

public class RankingCommandTests
{
    private RankingCommand _rankingCommand;
    private IShowdownRanksProvider _showdownRanksProvider;
    private ITemplatesManager _templatesManager;
    private IFormatsManager _formatsManager;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _showdownRanksProvider = Substitute.For<IShowdownRanksProvider>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _formatsManager = Substitute.For<IFormatsManager>();
        _context = Substitute.For<IContext>();
        _rankingCommand = new RankingCommand(_showdownRanksProvider, _templatesManager, _formatsManager);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithNoRatingsMessage_WhenNoRankingsAreFound()
    {
        // Arrange
        _context.Target.Returns("player1");
        _showdownRanksProvider.GetRankingDataAsync(Arg.Any<string>())
            .Returns(new List<RankingDataDto>());

        // Act
        await _rankingCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyRankAwareLocalizedMessage("rankcommand_no_ratings");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtml_WhenRankingsAreFound()
    {
        // Arrange
        var rankings = new List<RankingDataDto>
        {
            new() { FormatId = "gen8ou", Gxe = 90 },
            new() { FormatId = "gen7ou", Gxe = 85 }
        };
        _context.Target.Returns("player1");
        _showdownRanksProvider.GetRankingDataAsync(Arg.Any<string>())
            .Returns(rankings);
        _formatsManager.GetCleanFormat(Arg.Any<string>())
            .Returns(x => x[0]); // Return the same format for simplicity
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<RankingShowcaseViewModel>())
            .Returns(Task.FromResult("formatted_html"));

        // Act
        await _rankingCommand.RunAsync(_context);

        // Assert
        await _templatesManager.Received()
            .GetTemplateAsync("Showdown/Ranking/RankingShowcase", Arg.Any<RankingShowcaseViewModel>());
        _context.Received().SendHtmlIn("formatted_html", rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendLowestRankings_WhenCommandIsLowestRank()
    {
        // Arrange
        var rankings = new List<RankingDataDto>
        {
            new() { FormatId = "gen4ou", Gxe = 70 },
            new() { FormatId = "gen5ou", Gxe = 65 },
            new() { FormatId = "gen1ou", Gxe = 85 },
            new() { FormatId = "gen3ou", Gxe = 75 },
            new() { FormatId = "gen8ou", Gxe = 50 },
            new() { FormatId = "gen2ou", Gxe = 80 },
            new() { FormatId = "gen6ou", Gxe = 60 },
            new() { FormatId = "gen7ou", Gxe = 55 },
        };
        _formatsManager.GetCleanFormat(Arg.Any<string>()).Returns(t => t.Args()[0]);
        _context.Target.Returns("player1");
        _context.Command.Returns("lowestrank");
        _showdownRanksProvider.GetRankingDataAsync(Arg.Any<string>())
            .Returns(rankings);
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<RankingShowcaseViewModel>())
            .Returns(Task.FromResult("formatted_html"));

        // Act
        await _rankingCommand.RunAsync(_context);

        // Assert
        string[] expectedFormats = ["gen4ou", "gen5ou", "gen6ou", "gen7ou", "gen8ou"];
        await _templatesManager.Received()
            .GetTemplateAsync("Showdown/Ranking/RankingShowcase",
                Arg.Is<RankingShowcaseViewModel>(vm =>
                    vm.Rankings.Select(r => r.FormatId)
                        .SequenceEqual(expectedFormats)));
        _context.Received().SendHtmlIn("formatted_html", rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenExceptionOccurs()
    {
        // Arrange
        _context.Target.Returns("player1");
        _showdownRanksProvider.GetRankingDataAsync(Arg.Any<string>())
            .Throws(new Exception("API failure"));

        // Act
        await _rankingCommand.RunAsync(_context);

        // Assert
        _context.Received().ReplyRankAwareLocalizedMessage("rankcommand_error");
    }
}