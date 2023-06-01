using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class DeleteCustomCommand : ICommand
{
    public static string Name => "delete-custom-command";

    public static IEnumerable<string> Aliases =>
        new[] { "deletecustom", "deletecommand", "delete-custom", "delete-command" };

    public char RequiredRank => '%';
    public string HelpMessageKey => "deletecommand_help";

    private readonly IRepository<AddedCommand, Tuple<string, string>> _addedCommandRepository;

    public DeleteCustomCommand(IRepository<AddedCommand, Tuple<string, string>> addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }

    public async Task Run(IContext context)
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