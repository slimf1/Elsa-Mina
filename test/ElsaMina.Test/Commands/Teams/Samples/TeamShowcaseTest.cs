using System.Globalization;
using ElsaMina.Commands.Teams;
using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Commands.Teams.Samples;

public class TeamShowcaseTests
{
    private TeamShowcase _command;
    private ITeamRepository _teamRepository;
    private ITemplatesManager _templatesManager;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _teamRepository = Substitute.For<ITeamRepository>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _command = new TeamShowcase(_teamRepository, _templatesManager);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithNotFoundMessage_WhenTeamDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("nonexistentTeamId");
        _teamRepository.GetByIdAsync("nonexistentteamid").Returns(Task.FromResult<Team>(null));

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
        _context.Target.Returns("teamId");
        _context.Culture.Returns(new CultureInfo("fr-FR"));
        _teamRepository.GetByIdAsync("teamid").Returns(Task.FromResult(team));

        var expectedHtml = "<div>Sample Team HTML</div>";
        _templatesManager.GetTemplateAsync("Teams/SampleTeam", Arg.Any<SampleTeamViewModel>())
            .Returns(Task.FromResult(expectedHtml));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync("Teams/SampleTeam", Arg.Is<SampleTeamViewModel>(vm =>
            vm.Culture.Name == "fr-FR" && vm.Team == team));
        _context.Received().ReplyHtml(expectedHtml.RemoveNewlines(), rankAware: true);
    }
}