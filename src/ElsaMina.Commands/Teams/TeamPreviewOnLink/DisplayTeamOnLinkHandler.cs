using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Teams.TeamPreviewOnLink;

public class DisplayTeamOnLinkHandler : ChatMessageHandler
{
    private const int USER_DELAY = 30;

    private readonly Dictionary<string, DateTimeOffset> _lastTeamTimes = new();

    private readonly IClockService _clockService;
    private readonly ITeamLinkMatchFactory _teamLinkMatchFactory;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;

    public DisplayTeamOnLinkHandler(IContextFactory contextFactory,
        IClockService clockService,
        ITeamLinkMatchFactory teamLinkMatchFactory,
        ITemplatesManager templatesManager,
        IRoomsManager roomManager)
        : base(contextFactory)
    {
        _clockService = clockService;
        _teamLinkMatchFactory = teamLinkMatchFactory;
        _templatesManager = templatesManager;
        _roomsManager = roomManager;
    }
    
    public override string Identifier => nameof(DisplayTeamOnLinkHandler);

    protected override async Task HandleMessage(IContext context)
    {
        var isShowingTeamLinksPreviewEnabled = _roomsManager.GetRoomBotConfigurationParameterValue(
            context.RoomId, RoomParametersConstants.IS_SHOWING_TEAM_LINKS_PREVIEW).ToBoolean();
        if (!isShowingTeamLinksPreviewEnabled)
        {
            return;
        }

        var now = _clockService.CurrentUtcDateTime;
        if (_lastTeamTimes.TryGetValue(context.Sender.UserId, out var userLastTeamTime)
            && (now - userLastTeamTime).TotalSeconds < USER_DELAY && !context.IsSenderWhitelisted)
        {
            return;
        }

        _lastTeamTimes[context.Sender.UserId] = now;

        var teamLinkMatch = _teamLinkMatchFactory.FindTeamLinkMatch(context.Message);
        if (teamLinkMatch == null)
        {
            return;
        }
        var sharedTeam = await teamLinkMatch.GetTeamExport();
        if (sharedTeam == null)
        {
            Logger.Current.Error("An error occurred while fetching team from link {0}",
                context.Message);
            return;
        }

        var template = await _templatesManager.GetTemplate("Teams/TeamPreview", new TeamPreviewViewModel
        {
            Author = sharedTeam.Author,
            Culture = context.Culture,
            Sender = context.Sender.Name,
            Team = ShowdownTeams.DeserializeTeamExport(sharedTeam.TeamExport)
        });

        context.SendHtml(template.RemoveNewlines());
    }
}