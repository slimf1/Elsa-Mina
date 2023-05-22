using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.DependencyInjection;
using Serilog;

namespace ElsaMina.Core.Services.Commands;

public class CommandExecutor : ICommandExecutor
{
    private readonly ILogger _logger;
    private readonly IDependencyContainerService _dependencyContainerService;

    public CommandExecutor(ILogger logger,
        IDependencyContainerService dependencyContainerService)
    {
        _logger = logger;
        _dependencyContainerService = dependencyContainerService;
    }

    public bool HasCommand(string commandName)
    {
        return _dependencyContainerService.IsCommandRegistered(commandName);
    }
    
    public async Task TryExecuteCommand(string commandName, IContext context)
    {
        var commandInstance = _dependencyContainerService.ResolveCommand<ICommand>(commandName);
        if (commandInstance == null)
        {
            _logger.Error("Could not find command with name {0}", commandName);
            throw new Exception("Could not find command"); // TODO: custom exc ?
        }
        await commandInstance.Call(context);
    }
}