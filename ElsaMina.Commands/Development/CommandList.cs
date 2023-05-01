using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Commands.Development;

public class CommandList : ICommand
{
    private readonly IDependencyContainerService _dependencyContainerService;

    public CommandList(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public static string Name => "commandlist";
    public static IEnumerable<string> Aliases => new[] { "allcommands", "commands" };
    public Task Run(Context context)
    {
        return Task.CompletedTask;
    }
}