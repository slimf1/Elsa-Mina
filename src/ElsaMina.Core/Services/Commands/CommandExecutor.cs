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

    public IEnumerable<ICommand> GetAllCommands()
    {
        return _dependencyContainerService
            .GetAllRegistrations<ICommand>()
            .DistinctBy(command => command.Name);
    }

    public async Task TryExecuteCommandAsync(string commandName, IContext context,
        CancellationToken cancellationToken = default)
    {
        if (_dependencyContainerService.IsRegisteredWithName<ICommand>(commandName))
        {
            Log.Information("Executing {0} as a normal command", commandName);
            var command = _dependencyContainerService.ResolveNamed<ICommand>(commandName);

            if (CanCommandBeRan(context, command))
            {
                await command.RunAsync(context, cancellationToken);
                return;
            }

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

    private static bool CanCommandBeRan(IContext context, ICommand command)
    {
        if (command.IsPrivateMessageOnly && !context.IsPrivateMessage)
        {
            return false;
        }

        if (context.IsPrivateMessage && !(command.IsAllowedInPrivateMessage || command.IsPrivateMessageOnly))
        {
            return false;
        }

        if (command.IsWhitelistOnly && !context.IsSenderWhitelisted)
        {
            return false;
        }

        if (!context.HasSufficientRank(command.RequiredRank))
        {
            return false;
        }

        if (command.RoomRestriction.Any() && !command.RoomRestriction.Contains(context.RoomId))
        {
            return false;
        }

        return true;
    }
}