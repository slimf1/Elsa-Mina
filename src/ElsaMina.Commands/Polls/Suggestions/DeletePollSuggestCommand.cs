using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;

namespace ElsaMina.Commands.Polls.Suggestions;

[NamedCommand("deletesuggpoll")]
public class DeletePollSuggestCommand : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public DeletePollSuggestCommand(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "deletesuggpoll_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(context.Target?.Trim(), out var suggestionId))
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var suggestion = await dbContext.PollSuggestions.FindAsync([suggestionId], cancellationToken);

        if (suggestion == null)
        {
            context.ReplyLocalizedMessage("deletesuggpoll_not_found", suggestionId);
            return;
        }

        dbContext.PollSuggestions.Remove(suggestion);
        await dbContext.SaveChangesAsync(cancellationToken);
        context.ReplyLocalizedMessage("deletesuggpoll_success", suggestionId);
    }
}
