using System.Globalization;
using ElsaMina.Commands.Teams;
using ElsaMina.Commands.Teams.TeamPreviewOnLink;
using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using NSubstitute;

namespace ElsaMina.Test.Commands.Teams.TeamPreviewOnLink;

public class DisplayTeamOnLinkHandlerTest
{
    private DisplayTeamOnLinkHandler _handler;
    private IClockService _clockService;
    private ITeamLinkMatchFactory _teamLinkMatchFactory;
    private ITemplatesManager _templatesManager;
    private IRoomsManager _roomsManager;
    private IConfigurationManager _configurationManager;
    private IContextFactory _contextFactory;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _clockService = Substitute.For<IClockService>();
        _teamLinkMatchFactory = Substitute.For<ITeamLinkMatchFactory>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _contextFactory = Substitute.For<IContextFactory>();

        _handler = new DisplayTeamOnLinkHandler(_contextFactory, _clockService, _teamLinkMatchFactory,
            _templatesManager, _roomsManager, _configurationManager);
        _context = Substitute.For<IContext>();
        _contextFactory.TryBuildContextFromReceivedMessage(Arg.Any<string[]>(), Arg.Any<string>())
            .Returns(_context);
    }

    [Test]
    public async Task Test_HandleMessage_ShouldNotProceed_WhenMessageStartsWithTrigger()
    {
        // Arrange
        _configurationManager.Configuration.Trigger.Returns("!");
        _context.Message.Returns("!triggered");

        // Act
        await _handler.OnMessageReceived(null);

        // Assert
        _teamLinkMatchFactory.DidNotReceive().FindTeamLinkMatch(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleMessage_ShouldNotProceed_WhenSenderIsBot()
    {
        // Arrange
        _configurationManager.Configuration.Name.Returns("BotName");
        _context.Sender.UserId.Returns("botname");
        _context.Message.Returns("some message");

        // Act
        await _handler.OnMessageReceived(null);

        // Assert
        _teamLinkMatchFactory.DidNotReceive().FindTeamLinkMatch(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleMessage_ShouldNotProceed_WhenTeamLinksPreviewDisabled()
    {
        // Arrange
        _roomsManager.GetRoomBotConfigurationParameterValue(Arg.Any<string>(), RoomParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW)
            .Returns("false");

        // Act
        await _handler.OnMessageReceived(null);

        // Assert
        _teamLinkMatchFactory.DidNotReceive().FindTeamLinkMatch(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleMessage_ShouldThrottleUser_WhenCalledTooSoon()
    {
        // Arrange
        _clockService.CurrentUtcDateTime.Returns(DateTime.UtcNow);
        _context.Sender.UserId.Returns("user1");

        // Act
        await _handler.OnMessageReceived(null);

        // Assert
        _teamLinkMatchFactory.DidNotReceive().FindTeamLinkMatch(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleMessage_ShouldNotProceed_WhenTeamLinkNotMatched()
    {
        // Arrange
        _context.Message.Returns("some message");
        _teamLinkMatchFactory.FindTeamLinkMatch("some message").Returns((ITeamLinkMatch)null);

        // Act
        await _handler.OnMessageReceived(null);

        // Assert
        await _templatesManager.DidNotReceive().GetTemplate(Arg.Any<string>(), Arg.Any<TeamPreviewViewModel>());
    }

    [Test]
    public async Task Test_HandleMessage_ShouldSendHtml_WhenTeamLinkMatched()
    {
        // Arrange
        _context.Message.Returns("team link");
        _context.Culture.Returns(new CultureInfo("en-US"));
        _context.Sender.UserId.Returns("userId");
        _context.Sender.Name.Returns("User");
        _context.RoomId.Returns("roomId");
        var teamLinkMatch = Substitute.For<ITeamLinkMatch>();
        var sharedTeam = new SharedTeam { Author = "Author", TeamExport = "Export" };
        teamLinkMatch.GetTeamExport().Returns(Task.FromResult(sharedTeam));
        _roomsManager.GetRoomBotConfigurationParameterValue(
            "roomId", RoomParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW).Returns("true");
        _teamLinkMatchFactory.FindTeamLinkMatch("team link").Returns(teamLinkMatch);
        _configurationManager.Configuration.Returns(new Configuration { Trigger = "-", Name = "Bot" });

        var expectedHtml = "<div>Team Preview HTML</div>";
        _templatesManager.GetTemplate("Teams/TeamPreview", Arg.Any<TeamPreviewViewModel>())
            .Returns(Task.FromResult(expectedHtml));

        // Act
        await _handler.OnMessageReceived(null);

        // Assert
        await _templatesManager.Received(1).GetTemplate("Teams/TeamPreview", Arg.Is<TeamPreviewViewModel>(vm =>
            vm.Author == "Author" && vm.Culture.Name == "en-US" && vm.Sender == "User" && vm.Team != null));
        _context.Received().SendHtml(expectedHtml.RemoveNewlines());
    }
}