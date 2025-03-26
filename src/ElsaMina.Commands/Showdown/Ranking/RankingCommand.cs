using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Showdown.Ranking;

[NamedCommand("rank", "ranking", "lowestrank", "lowest-rank")]
public class RankingCommand : Command
{
    private const int RANKS_SHOWN_COUNT = 5;

    private readonly IShowdownRanksProvider _showdownRanksProvider;
    private readonly ITemplatesManager _templatesManager;
    private readonly IFormatsManager _formatsManager;

    public RankingCommand(IShowdownRanksProvider showdownRanksProvider,
        ITemplatesManager templatesManager,
        IFormatsManager formatsManager)
    {
        _showdownRanksProvider = showdownRanksProvider;
        _templatesManager = templatesManager;
        _formatsManager = formatsManager;
    }

    public override bool IsAllowedInPrivateMessage => true;

    public override string HelpMessageKey => "rankcommand_help";

    public override Rank RequiredRank => Rank.Regular;

    public override async Task Run(IContext context)
    {
        var username = string.IsNullOrWhiteSpace(context.Target)
            ? context.Sender.Name
            : context.Target;
        var isReversed = context.Command is "lowestrank" or "lowest-rank";

        try
        {
            var result = await _showdownRanksProvider.GetRankingDataAsync(username.ToLowerAlphaNum());
            var sortedRankings = result.OrderByDescending(rankingDto => rankingDto.Gxe).ToList();

            sortedRankings.ForEach(rankingDto =>
                rankingDto.FormatId = _formatsManager.GetCleanFormat(rankingDto.FormatId));

            if (sortedRankings.Count == 0)
            {
                context.ReplyRankAwareLocalizedMessage("rankcommand_no_ratings");
                return;
            }

            var template = await _templatesManager.GetTemplate("Showdown/Ranking/RankingShowcase",
                new RankingShowcaseViewModel
                {
                    Culture = context.Culture,
                    Username = username,
                    Rankings = isReversed
                        ? sortedRankings.TakeLast(RANKS_SHOWN_COUNT)
                        : sortedRankings.Take(RANKS_SHOWN_COUNT)
                }
            );

            context.SendHtml(template.RemoveNewlines(), rankAware: true);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Ranking command failed");
            context.ReplyRankAwareLocalizedMessage("rankcommand_error");
        }
    }
}