using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("custom-command-list", Aliases =
[
    "added-command-list", "added-commands", "custom-commands",
    "addedcommands", "customcommands", "commandslist", "commandlist", "customcommandlist", "customs-list"
])]
public class CustomCommandList : Command
{
    private readonly IAddedCommandRepository _addedCommandRepository;

    public CustomCommandList(IAddedCommandRepository addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var addedCommands = (await _addedCommandRepository.GetAllAsync())
            .Where(command => command.RoomId == context.RoomId)
            .Select(command => command.Id);
        context.Reply($"**Commands**: {string.Join(", ", addedCommands)}");
    }
}