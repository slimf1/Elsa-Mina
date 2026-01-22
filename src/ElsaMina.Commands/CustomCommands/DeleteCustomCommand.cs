using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("delete-custom-command", Aliases = ["deletecustom", "deletecommand", "delete-custom", "delete-command"])]
public class DeleteCustomCommand : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public DeleteCustomCommand(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "deletecommand_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var commandId = context.Target.Trim().ToLower();
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var existingCommand = await dbContext.AddedCommands
                .FindAsync([commandId, context.RoomId], cancellationToken);
            if (existingCommand == null)
            {
                context.ReplyLocalizedMessage("deletecommand_not_found", commandId);
                return;
            }

            dbContext.AddedCommands.Remove(existingCommand);
            await dbContext.SaveChangesAsync(cancellationToken);
            context.ReplyLocalizedMessage("deletecommand_success", commandId);
        }
        catch (Exception exception)
        {
            context.ReplyLocalizedMessage("deletecommand_failure", exception.Message);
        }
    }
}