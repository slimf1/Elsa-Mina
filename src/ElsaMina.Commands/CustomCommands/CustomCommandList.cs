using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class CustomCommandList : Command<CustomCommandList>, INamed
{
    public static string Name => "custom-command-list";
    public static List<string> Aliases =>
    [
        "added-command-list", "added-commands", "custom-commands", "addedcommands", "customcommands", "commandslist",
        "commandlist", "customcommandlist", "customs-list"
    ];

    private readonly IAddedCommandRepository _addedCommandRepository;

    public CustomCommandList(IAddedCommandRepository addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }
    
    public override char RequiredRank => '+';

    public override async Task Run(IContext context)
    {
        var addedCommands = (await _addedCommandRepository.GetAllAsync())
            .Where(command => command.RoomId == context.RoomId)
            .Select(command => command.Id);
        context.Reply($"**Commands**: {string.Join(", ", addedCommands)}");
    }
}