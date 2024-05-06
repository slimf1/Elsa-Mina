using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core.Services.Commands;

public class CommandExecutor : ICommandExecutor
{
    private readonly IDependencyContainerService _dependencyContainerService;
    private readonly IAddedCommandsManager _addedCommandsManager;

    public CommandExecutor(IDependencyContainerService dependencyContainerService,
        IAddedCommandsManager addedCommandsManager)
    {
        _dependencyContainerService = dependencyContainerService;
        _addedCommandsManager = addedCommandsManager;
    }

    public bool HasCommand(string commandName)
    {
        return _dependencyContainerService.IsCommandRegistered(commandName);
    }

    public IEnumerable<ICommand> GetAllCommands()
    {
        return _dependencyContainerService.GetAllCommands();
    }

    public Task OnBotStartUp()
    {
        return Task.WhenAll(GetAllCommands().Select(command => command.OnBotStartUp()));
    }

    public async Task TryExecuteCommand(string commandName, IContext context)
    {
        if (HasCommand(commandName))
        {
            Logger.Current.Information("Executing {0} as a normal command", commandName);
            var commandInstance = _dependencyContainerService.ResolveCommand<ICommand>(commandName);
            await commandInstance.Call(context);
            return;
        }
        
        if (!context.IsPm) {
            Logger.Current.Information("Trying command {0} as a custom command", commandName);
            await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);
            return;
        }
        
        Logger.Current.Error("Could not find command {0}", commandName);
    }
}