using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;

namespace ElsaMina.Commands.Arcade.Points;

[NamedCommand("clearpoints")]
public class ClearPointsCommand : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public ClearPointsCommand(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "clear_points_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.UserPoints.RemoveRange(dbContext.UserPoints);
        await dbContext.SaveChangesAsync(cancellationToken);

        context.ReplyLocalizedMessage("clear_points_success");
    }
}