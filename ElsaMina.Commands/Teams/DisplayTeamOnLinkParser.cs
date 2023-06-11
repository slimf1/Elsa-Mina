using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates.TeamPreview;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Commands.Teams;

public class DisplayTeamOnLinkParser : ChatMessageParser
{
    private const int USER_DELAY = 30;
    
    private readonly Dictionary<string, DateTimeOffset> _lastTeamTimes = new();

    private readonly ILogger _logger;
    private readonly IClockService _clockService;
    private readonly ITeamProviderFactory _teamProviderFactory;
    private readonly ITemplatesManager _templatesManager;
    private readonly IRoomsManager _roomsManager;
    
    public DisplayTeamOnLinkParser(ILogger logger,
        IDependencyContainerService dependencyContainerService,
        IClockService clockService,
        ITeamProviderFactory teamProviderFactory,
        ITemplatesManager templatesManager,
        IRoomsManager roomsManager)
        : base(dependencyContainerService)
    {
        _clockService = clockService;
        _teamProviderFactory = teamProviderFactory;
        _templatesManager = templatesManager;
        _roomsManager = roomsManager;
        _logger = logger;
    }

    protected override async Task HandleChatMessage(IContext context)
    {
        if (!_teamProviderFactory.SupportedProviderLinks.Any(providerLink => context.Target.Contains(providerLink)))
        {
            return;
        }

        var room = _roomsManager.GetRoom(context.RoomId);
        if ((room.RoomParameters?.IsShowingTeamLinksPreviews ?? false) == false)
        {
            return;
        }

        var now = _clockService.CurrentDateTimeOffset;
        if (_lastTeamTimes.TryGetValue(context.Sender.UserId, out var userLastTeamTime)
            && (now - userLastTeamTime).TotalSeconds < USER_DELAY && !context.IsSenderWhitelisted)
        {
            return;
        }

        _lastTeamTimes[context.Sender.UserId] = now;

        var match = TeamProviderFactory.TEAM_LINK_REGEX.Match(context.Target);
        if (!match.Success)
        {
            return;
        }

        var link = match.Value;
        var provider = _teamProviderFactory.GetTeamProvider(link);
        var sharedTeam = await provider.GetTeamExport(link);
        if (sharedTeam == null)
        {
            _logger.Error("An error occurred while fetching team from link {0} with provider {1}",
                link, provider);
            return;
        }

        var template = await _templatesManager.GetTemplate("TeamPreview/TeamPreview", new TeamPreviewViewModel
        {
            Author = sharedTeam.Author,
            Culture = context.Locale,
            Sender = context.Sender.Name,
            Team = ShowdownTeams.DeserializeTeamExport(sharedTeam.TeamExport)
        });
        
        context.SendHtml(template.RemoveNewlines());
    }
}