using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("delete-custom-command", Aliases = ["deletecustom", "deletecommand", "delete-custom", "delete-command"])]
public class DeleteCustomCommand : Command
{
    private readonly IAddedCommandRepository _addedCommandRepository;

    public DeleteCustomCommand(IAddedCommandRepository addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "deletecommand_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var commandId = context.Target.Trim().ToLower();
        try
        {
            await _addedCommandRepository.DeleteByIdAsync(Tuple.Create(commandId, context.RoomId), cancellationToken);
            context.ReplyLocalizedMessage("deletecommand_success", commandId);
        }
        catch (Exception exception)
        {
            context.ReplyLocalizedMessage("deletecommand_failure", exception.Message);
        }
    }
}