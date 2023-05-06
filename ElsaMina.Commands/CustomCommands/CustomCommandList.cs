using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class CustomCommandList : ICommand
{
    public static string Name => "custom-command-list";
    public static IEnumerable<string> Aliases => new[] { "added-command-list", "added-commands", "custom-commands" };
    public char RequiredRank => '+';

    private readonly IAddedCommandRepository _addedCommandRepository;

    public CustomCommandList(IAddedCommandRepository addedCommandRepository)
    {
        _addedCommandRepository = addedCommandRepository;
    }

    public async Task Run(Context context)
    {
        var addedCommands = await _addedCommandRepository.GetAllAsync();
        context.Reply($"**Commands**: {string.Join(", ", addedCommands.Select(command => command.Id))}");
    }
}