using System.Globalization;
using ElsaMina.Commands.Teams;
using ElsaMina.Commands.Teams.Tournaments;
using ElsaMina.Core;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Commands.Teams.Tournaments;

public class DisplayTeamsOnTourHandlerTest
{
    private ITeamRepository _teamRepository;
    private ITemplatesManager _templatesManager;
    private IRoomsManager _roomsManager;
    private IBot _bot;
    private DisplayTeamsOnTourHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _teamRepository = Substitute.For<ITeamRepository>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _bot = Substitute.For<IBot>();

        _handler = new DisplayTeamsOnTourHandler(_teamRepository, _templatesManager, _roomsManager, _bot);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotHandle_WhenInvalidParts()
    {
        // Act
        await _handler.HandleReceivedMessage(["invalid", "message"], "roomId");

        // Assert
        await _teamRepository.DidNotReceive().GetTeamsFromRoomWithFormat(Arg.Any<string>(), Arg.Any<string>());
        await _templatesManager.DidNotReceive().GetTemplate(Arg.Any<string>(), Arg.Any<object>());
        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotHandle_WhenFormatContainsRandom()
    {
        // Act
        await _handler.HandleReceivedMessage(["", "tournament", "create", "randomformat"], "roomId");

        // Assert
        await _teamRepository.DidNotReceive().GetTeamsFromRoomWithFormat(Arg.Any<string>(), Arg.Any<string>());
        await _templatesManager.DidNotReceive().GetTemplate(Arg.Any<string>(), Arg.Any<object>());
        _bot.DidNotReceive().Say(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNormalizeAndAliasFormatCorrectly_WhenValidGen9FormatProvided()
    {
        // Arrange
        List<Team> teams = [
            new() { Id = "team1" },
            new() { Id = "team2" }
        ];
        var room = Substitute.For<IRoom>();
        room.Culture.Returns(new CultureInfo("fr-FR"));
        _roomsManager.GetRoom("roomId").Returns(room);
        _teamRepository.GetTeamsFromRoomWithFormat("roomId", "natdex").Returns(teams);
        _templatesManager.GetTemplate("Teams/TeamList", Arg.Any<TeamListViewModel>())
            .Returns("renderedTemplate");

        // Act
        await _handler.HandleReceivedMessage(["", "tournament", "create", "gen9nationaldex"], "roomId");

        // Assert
        await _teamRepository.Received(1).GetTeamsFromRoomWithFormat("roomId", "natdex");
        await _templatesManager.Received(1).GetTemplate("Teams/TeamList",
            Arg.Is<TeamListViewModel>(vm => vm.Culture.Name == "fr-FR" && vm.Teams.SequenceEqual(teams)));
        _bot.Received(1).Say("roomId", "/addhtmlbox renderedTemplate");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldHandleAliasFormatsCorrectly_WhenFormatIsAnAlias()
    {
        // Arrange
        List<Team> teams = [
            new() { Id = "team1" },
            new() { Id = "team2" }
        ];
        var room = Substitute.For<IRoom>();
        room.Culture.Returns(new CultureInfo("fr-FR"));
        _roomsManager.GetRoom("roomId").Returns(room);
        _teamRepository.GetTeamsFromRoomWithFormat("roomId", "aaa").Returns(teams);
        _templatesManager.GetTemplate("Teams/TeamList", Arg.Any<TeamListViewModel>())
            .Returns("renderedTemplate");

        // Act
        await _handler.HandleReceivedMessage(["", "tournament", "create", "almostanyability"], "roomId");

        // Assert
        await _teamRepository.Received(1).GetTeamsFromRoomWithFormat("roomId", "aaa");
        await _templatesManager.Received(1).GetTemplate("Teams/TeamList",
            Arg.Is<TeamListViewModel>(vm => vm.Culture.Name == "fr-FR" && vm.Teams.SequenceEqual(teams)));
        _bot.Received(1).Say("roomId", "/addhtmlbox renderedTemplate");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldSendTemplate_WhenFormatIsValid()
    {
        // Arrange
        var teams = new List<Team>
        {
            new Team
            {
                Author = "Obama",
                Id = "teamId",
                Rooms = new List<RoomTeam>
                {
                    new()
                    {
                        TeamId = "teamId",
                        RoomId = "franais"
                    }
                }
            }
        };
        var room = Substitute.For<IRoom>();
        room.Culture.Returns(new CultureInfo("fr-FR"));
        _roomsManager.GetRoom("roomId").Returns(room);
        _teamRepository.GetTeamsFromRoomWithFormat("roomId", "ag").Returns(teams);
        _templatesManager.GetTemplate("Teams/TeamList", Arg.Any<TeamListViewModel>())
            .Returns("renderedTemplate");

        // Act
        await _handler.HandleReceivedMessage(["", "tournament", "create", "anythinggoes"], "roomId");

        // Assert
        await _teamRepository.Received(1).GetTeamsFromRoomWithFormat("roomId", "ag");
        await _templatesManager.Received(1).GetTemplate("Teams/TeamList",
            Arg.Is<TeamListViewModel>(vm => vm.Culture.Name == "fr-FR" && vm.Teams.SequenceEqual(teams)));
        _bot.Received(1).Say("roomId", "/addhtmlbox renderedTemplate");
    }
}