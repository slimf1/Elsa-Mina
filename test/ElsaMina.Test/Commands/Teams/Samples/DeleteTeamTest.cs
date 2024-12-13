using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Commands.Teams.Samples;

public class DeleteTeamTests
{
    private DeleteTeam _command;
    private ITeamRepository _teamRepository;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _teamRepository = Substitute.For<ITeamRepository>();
        _command = new DeleteTeam(_teamRepository);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithTeamNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        _context.Target.Returns("nonexistentTeamId");
        _teamRepository.GetByIdAsync("nonexistentteamid").Returns(Task.FromResult<Team>(null));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("deleteteam_team_not_found");
    }

    [Test]
    public async Task Test_Run_ShouldDeleteTeamAndReplyWithSuccess_WhenTeamExists()
    {
        // Arrange
        var team = new Team { Id = "teamid" };
        _context.Target.Returns("teamId");
        _teamRepository.GetByIdAsync("teamid").Returns(Task.FromResult(team));

        // Act
        await _command.Run(_context);

        // Assert
        await _teamRepository.Received(1).DeleteByIdAsync("teamid");
        _context.Received().ReplyLocalizedMessage("deleteteam_team_deleted_successfully");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithDeletionError_WhenRepositoryThrowsException()
    {
        // Arrange
        var team = new Team { Id = "teamid" };
        _context.Target.Returns("teamId");
        _teamRepository.GetByIdAsync("teamid").Returns(Task.FromResult(team));
        _teamRepository.When(repo => repo.DeleteByIdAsync("teamid")).Do(_ => throw new Exception("Deletion error"));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("deleteteam_team_deletion_error", "Deletion error");
    }
}