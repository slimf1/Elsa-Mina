using ElsaMina.Commands.Teams.Samples;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Commands.Teams.Samples;

public class AddTeamToRoomTests
{
    private AddTeamToRoom _command;
    private ITeamRepository _teamRepository;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _teamRepository = Substitute.For<ITeamRepository>();
        _command = new AddTeamToRoom(_teamRepository);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithNoArgMessage_WhenTeamIdIsEmpty()
    {
        // Arrange
        _context.Target.Returns(string.Empty);

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_to_room_no_arg");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithNoTeamMessage_WhenTeamNotFound()
    {
        // Arrange
        _context.Target.Returns("teamId");
        _teamRepository.GetByIdAsync("teamid").Returns(Task.FromResult<Team>(null));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_to_room_no_team");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithAlreadyInRoomMessage_WhenTeamAlreadyInRoom()
    {
        // Arrange
        _context.Target.Returns("teamId");
        _context.RoomId.Returns("roomId");

        var team = new Team
        {
            Id = "teamid",
            Rooms = new List<RoomTeam> { new RoomTeam { RoomId = "roomId", TeamId = "teamid" } }
        };
        _teamRepository.GetByIdAsync("teamid").Returns(team);

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_to_room_team_already_in_room");
    }

    [Test]
    public async Task Test_Run_ShouldAddRoomAndReplyWithSuccess_WhenTeamIsValidAndNotInRoom()
    {
        // Arrange
        _context.Target.Returns("teamId");
        _context.RoomId.Returns("roomId");

        var team = new Team
        {
            Id = "teamid",
            Rooms = new List<RoomTeam>()
        };
        _teamRepository.GetByIdAsync("teamid").Returns(team);

        // Act
        await _command.Run(_context);

        // Assert
        Assert.That(team.Rooms.Any(roomTeam => roomTeam.RoomId == "roomId" && roomTeam.TeamId == "teamid"), Is.True);
        await _teamRepository.Received(1).UpdateAsync(team);
        _context.Received().ReplyLocalizedMessage("add_team_to_room_success");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithFailureMessage_WhenRepositoryUpdateThrowsException()
    {
        // Arrange
        _context.Target.Returns("teamId");
        _context.RoomId.Returns("roomId");

        var team = new Team
        {
            Id = "teamid",
            Rooms = new List<RoomTeam>()
        };
        _teamRepository.GetByIdAsync("teamid").Returns(team);
        _teamRepository.When(repo => repo.UpdateAsync(team)).Do(_ => throw new Exception("Update error"));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("add_team_to_room_failure", "Update error");
    }
}
