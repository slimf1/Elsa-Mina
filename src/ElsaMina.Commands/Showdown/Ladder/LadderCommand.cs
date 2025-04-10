using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Showdown.Ladder;

[NamedCommand("ladder")]
public class LadderCommand : Command
{
    private const string LADDER_RESOURCE_URL = "https://pokemonshowdown.com/ladder/{0}.json";
    private const int MAX_PLAYERS_WITHOUT_PREFIX = 200;

    private readonly IHttpService _httpService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IFormatsManager _formatsManager;

    public LadderCommand(IHttpService httpService,
        ITemplatesManager templatesManager,
        IFormatsManager formatsManager)
    {
        _httpService = httpService;
        _templatesManager = templatesManager;
        _formatsManager = formatsManager;
    }

    public override bool IsAllowedInPrivateMessage => true;

    public override string HelpMessageKey => "ladder_help";
    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var parts = context.Target.Split(",");
            var tier = parts[0].ToLowerAlphaNum();
            var prefix = parts.Length > 1 ? parts[1].Trim().ToLower() : string.Empty;
            var response = await _httpService.GetAsync<LadderDto>(string.Format(LADDER_RESOURCE_URL, tier),
                cancellationToken: cancellationToken);
            var hasPrefix = !string.IsNullOrWhiteSpace(prefix);

            if (response?.Data?.TopList == null)
            {
                context.ReplyLocalizedMessage("ladder_no_players");
                return;
            }

            var index = 0;
            var innerIndex = 0;
            var players = new List<LadderPlayerDto>();
            foreach (var player in response.Data.TopList)
            {
                index++;
                if (player.Username.ToLower().Trim().StartsWith(prefix))
                {
                    innerIndex++;
                    player.Index = index;
                    player.InnerIndex = innerIndex;
                    players.Add(player);
                }

                // Only show the top 200 when there's no prefix because the message becomes too big (> 100KB)
                if (!hasPrefix && index >= MAX_PLAYERS_WITHOUT_PREFIX)
                {
                    break;
                }
            }

            if (players.Count == 0)
            {
                context.ReplyLocalizedMessage("ladder_no_players");
                return;
            }

            var template = await _templatesManager.GetTemplateAsync("Showdown/Ladder/LadderTable",
                new LadderTableViewModel
                {
                    Culture = context.Culture,
                    ShowInnerRanking = hasPrefix,
                    Format = _formatsManager.GetCleanFormat(response.Data.Format),
                    TopList = players
                });

            context.ReplyHtml(template, rankAware: true);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to get Ladder");
            context.ReplyLocalizedMessage("ladder_error");
        }
    }
}