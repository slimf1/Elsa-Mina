using System.Text.RegularExpressions;
using ElsaMina.Core.Commands.Parsers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates.TeamPreview;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Commands.Teams;

public class DisplayTeamOnLinkParser : ChatMessageParser
{
    private static readonly Regex TEAM_LINK_REGEX = new(@"(https:\/\/pokepast\.es\/[0-9A-Fa-f]{16})|(https:\/\/www\.coupcritique\.fr\/entity\/teams\/\d+\/?)");
    private const int USER_DELAY = 30;
    
    private readonly Dictionary<string, DateTimeOffset> _lastTeamTimes = new();

    private readonly ILogger _logger;
    private readonly IClockService _clockService;
    private readonly ITeamProviderFactory _teamProviderFactory;
    private readonly ITemplatesManager _templatesManager;
    
    public DisplayTeamOnLinkParser(ILogger logger,
        IDependencyContainerService dependencyContainerService,
        IClockService clockService,
        ITeamProviderFactory teamProviderFactory,
        ITemplatesManager templatesManager)
        : base(dependencyContainerService)
    {
        _clockService = clockService;
        _teamProviderFactory = teamProviderFactory;
        _templatesManager = templatesManager;
        _logger = logger;
    }

    protected override async Task HandleChatMessage(IContext context)
    {
        if (!context.Target.Contains("pokepast.es/") && !context.Target.Contains("coupcritique.fr/entity/teams/"))
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

        var match = TEAM_LINK_REGEX.Match(context.Target);
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
        var sets = ShowdownTeams.DeserializeTeamExport(sharedTeam.TeamExport);

        var template = await _templatesManager.GetTemplate("TeamPreview/TeamPreview", new TeamPreviewViewModel
        {
            Author = sharedTeam.Author,
            Culture = context.Locale,
            Sender = context.Sender.Name,
            Team = sets
        });
        
        context.SendHtml(template.RemoveNewlines());
    }
}