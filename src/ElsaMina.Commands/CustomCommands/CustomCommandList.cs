using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class CustomCommandList : BaseCommand<CustomCommandList>
{
    private readonly IRepository<AddedCommand, Tuple<string, string>> _addedCommandRepository;

    public CustomCommandList(IRepository<AddedCommand, Tuple<string, string>> addedCommandRepository)
    {
        Name = "custom-command-list";
        Aliases = new[] { "added-command-list", "added-commands", "custom-commands",
            "addedcommands", "customcommands", "commandslist", "commandlist", "customcommandlist", "customs-list" };
        
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