using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Commands.Teams.Samples;

public class AddTeamTests
{
    private AddTeam _command;
    private ITeamLinkMatchFactory _teamLinkMatchFactory;
    private ITeamRepository _teamRepository;
    private IClockService _clockService;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _teamLinkMatchFactory = Substitute.For<ITeamLinkMatchFactory>();
        _teamRepository = Substitute.For<ITeamRepository>();
        _clockService = Substitute.For<IClockService>();
        _command = new AddTeam(_teamLinkMatchFactory, _teamRepository, _clockService);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithHelpMessage_WhenTargetIsInvalid()
    {
        // Arrange
        _context.Target.Returns("invalid_target");

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().Reply(Arg.Any<string>());
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithNameTooLongMessage_WhenNameExceedsMaxLength()
    {
        // Arrange
        _context.Target.Returns("link, " + new string('a', 71) + ", format");

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_name_too_long");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithNoProviderMessage_WhenTeamLinkMatchNotFound()
    {
        // Arrange
        _context.Target.Returns("link, name, format");
        _teamLinkMatchFactory.FindTeamLinkMatch("link").Returns((ITeamLinkMatch)null);

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_no_provider");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithNoExportErrorMessage_WhenTeamExportIsNull()
    {
        // Arrange
        _context.Target.Returns("link, name, format");
        var teamLinkMatch = Substitute.For<ITeamLinkMatch>();
        teamLinkMatch.GetTeamExport().Returns(Task.FromResult((SharedTeam)null));
        _teamLinkMatchFactory.FindTeamLinkMatch("link").Returns(teamLinkMatch);

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_no_export_error");
    }

    [Test]
    public async Task Test_Run_ShouldAddTeamToRepository_WhenDataIsValid()
    {
        // Arrange
        _context.Target.Returns("link, name, format");
        _context.RoomId.Returns("room");
        _context.Sender.Name.Returns("author");

        var teamLinkMatch = Substitute.For<ITeamLinkMatch>();
        var sharedTeam = new SharedTeam { TeamExport = "export_data" };
        teamLinkMatch.GetTeamExport().Returns(Task.FromResult(sharedTeam));
        _teamLinkMatchFactory.FindTeamLinkMatch("link").Returns(teamLinkMatch);

        var currentDateTime = DateTime.UtcNow;
        _clockService.CurrentUtcDateTime.Returns(currentDateTime);

        // Act
        await _command.Run(_context);

        // Assert
        await _teamRepository.Received(1).AddAsync(Arg.Is<Team>(team =>
            team.Id == "name".ToLowerAlphaNum() &&
            team.Name == "name" &&
            team.Author == "author" &&
            team.Link == "link" &&
            team.CreationDate == currentDateTime &&
            team.TeamJson == ShowdownTeams.TeamExportToJson("export_data") &&
            team.Format == "format" &&
            team.Rooms.Count == 1 &&
            team.Rooms.ElementAt(0).RoomId == "room" &&
            team.Rooms.ElementAt(0).TeamId == "name".ToLowerAlphaNum()
        ));
        _context.Received().ReplyLocalizedMessage("add_team_success", "name".ToLowerAlphaNum());
    }

    [Test]
    public async Task Test_Run_ShouldAddTeamToBothRooms_WhenRoomIdIsFrenchOrArcade()
    {
        // Arrange
        _context.Target.Returns("link, name, format");
        _context.RoomId.Returns("arcade");
        _context.Sender.Name.Returns("author");

        var teamLinkMatch = Substitute.For<ITeamLinkMatch>();
        var sharedTeam = new SharedTeam { TeamExport = "export_data" };
        teamLinkMatch.GetTeamExport().Returns(Task.FromResult(sharedTeam));
        _teamLinkMatchFactory.FindTeamLinkMatch("link").Returns(teamLinkMatch);

        var currentDateTime = DateTime.UtcNow;
        _clockService.CurrentUtcDateTime.Returns(currentDateTime);

        // Act
        await _command.Run(_context);

        // Assert
        await _teamRepository.Received(1).AddAsync(Arg.Is<Team>(team =>
            team.Rooms.Count == 2 &&
            team.Rooms.Any(rt => rt.RoomId == "arcade") &&
            team.Rooms.Any(rt => rt.RoomId == "franais")
        ));
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithFailureMessage_WhenRepositoryThrowsException()
    {
        // Arrange
        _context.Target.Returns("link, name, format");
        _context.RoomId.Returns("room");
        _context.Sender.Name.Returns("author");

        var teamLinkMatch = Substitute.For<ITeamLinkMatch>();
        var sharedTeam = new SharedTeam { TeamExport = "export_data" };
        teamLinkMatch.GetTeamExport().Returns(Task.FromResult(sharedTeam));
        _teamLinkMatchFactory.FindTeamLinkMatch("link").Returns(teamLinkMatch);

        _teamRepository.When(repo => repo.AddAsync(Arg.Any<Team>()))
            .Do(_ => throw new Exception("Database error"));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_failure", "Database error");
    }
}
