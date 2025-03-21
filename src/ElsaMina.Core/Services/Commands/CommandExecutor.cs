﻿using ElsaMina.Core.Commands;
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

    public IEnumerable<ICommand> GetAllCommands()
    {
        return _dependencyContainerService
            .GetAllRegistrations<ICommand>()
            .DistinctBy(command => command.Name);
    }

    public async Task TryExecuteCommand(string commandName, IContext context)
    {
        if (_dependencyContainerService.IsRegisteredWithName<ICommand>(commandName))
        {
            Log.Information("Executing {0} as a normal command", commandName);
            var commandInstance = _dependencyContainerService.ResolveNamed<ICommand>(commandName);
            await commandInstance.Call(context);
            return;
        }

        if (!context.IsPrivateMessage)
        {
            Log.Information("Trying command {0} as a custom command", commandName);
            await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);
            return;
        }

        Log.Error("Could not find command {0}", commandName);
    }
}