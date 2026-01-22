using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("custom-command-list", Aliases =
[
    "added-command-list", "added-commands", "custom-commands",
    "addedcommands", "customcommands", "commandslist", "commandlist", "customcommandlist", "customs-list"
])]
public class CustomCommandList : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public CustomCommandList(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var addedCommands = await dbContext.AddedCommands
            .Where(command => command.RoomId == context.RoomId)
            .Select(command => command.Id)
            .ToListAsync(cancellationToken);
        context.Reply($"**Commands**: {string.Join(", ", addedCommands)}");
    }
}