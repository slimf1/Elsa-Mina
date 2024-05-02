using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class EditCustomCommand : Command<EditCustomCommand>, INamed
{
    public static string Name => "edit-command";
    public static IEnumerable<string> Aliases => new[]
    {
        "edit-added-command", "edit-custom-command", "editcommand",
        "editcustom"
    };

    private readonly IAddedCommandRepository _addedCommandsRepository;

    public EditCustomCommand(IAddedCommandRepository addedCommandsRepository)
    {
        _addedCommandsRepository = addedCommandsRepository;
    }
    
    public override char RequiredRank => '%';
    public override string HelpMessageKey => "editcommand_help";

    public override async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        var commandId = parts[0].Trim().ToLower();
        var content = parts[1].Trim();
        AddedCommand command = null;
        try
        {
            command = await _addedCommandsRepository.GetByIdAsync(new(commandId, context.RoomId));
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "Error executing command");
        }

        if (command == null)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        command.Content = content;

        try
        {
            await _addedCommandsRepository.UpdateAsync(command);
            context.ReplyLocalizedMessage("editcommand_success", commandId);
        }
        catch (Exception exception)
        {
            context.ReplyLocalizedMessage("editcommand_failure", exception.Message);
        }
    }
}