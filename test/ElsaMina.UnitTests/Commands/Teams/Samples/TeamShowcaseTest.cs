using System.Globalization;
using ElsaMina.Commands.Teams;
using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Teams.Samples;

public class TeamShowcaseCommandTests
{
    private TeamShowcaseCommand _command;
    private IBotDbContextFactory _dbContextFactory;
    private ITemplatesManager _templatesManager;
    private IContext _context;
    private BotDbContext _dbContext;

    [SetUp]
    public void SetUp()
    {
        var dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new BotDbContext(dbOptions);

        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult<BotDbContext>(_dbContext));

        _templatesManager = Substitute.For<ITemplatesManager>();
        _command = new TeamShowcaseCommand(_templatesManager, _dbContextFactory);

        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithNotFoundMessage_WhenTeamDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("nonexistentTeamId");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("team_showcase_not_found");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtml_WhenTeamExists()
    {
        // Arrange
        var team = new Team { Id = "teamid", Name = "Test Team" };
        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();

        _context.Target.Returns("teamId");
        _context.Culture.Returns(new CultureInfo("fr-FR"));

        var expectedHtml = "<div>Sample Team HTML</div>";
        _templatesManager.GetTemplateAsync(
                "Teams/SampleTeam",
                Arg.Any<SampleTeamViewModel>())
            .Returns(Task.FromResult(expectedHtml));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Teams/SampleTeam",
            Arg.Is<SampleTeamViewModel>(vm =>
                vm.Team == team &&
                vm.Culture.Name == "fr-FR"));

        _context.Received().ReplyHtml(expectedHtml.RemoveNewlines(), rankAware: true);
    }
}
