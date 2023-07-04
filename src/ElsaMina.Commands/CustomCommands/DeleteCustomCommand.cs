using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class DeleteCustomCommand : BaseCommand<DeleteCustomCommand>
{
    private readonly IRepository<AddedCommand, Tuple<string, string>> _addedCommandRepository;

    public DeleteCustomCommand(IRepository<AddedCommand, Tuple<string, string>> addedCommandRepository)
    {
        Name = "delete-custom-command";
        Aliases = new[] { "deletecustom", "deletecommand", "delete-custom", "delete-command" };;
        
        _addedCommandRepository = addedCommandRepository;
    }

    public override char RequiredRank => '%';
    public override string HelpMessageKey => "deletecommand_help";
    
    public override async Task Run(IContext context)
    {
        var commandId = context.Target.Trim().ToLower();
        try
        {
            await _addedCommandRepository.DeleteAsync(new(commandId, context.RoomId));
            context.ReplyLocalizedMessage("deletecommand_success", commandId);
        }
        catch (Exception exception)
        {
            context.ReplyLocalizedMessage("deletecommand_failure", exception.Message);
        }
    }
}