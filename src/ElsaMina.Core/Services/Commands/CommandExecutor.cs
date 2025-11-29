using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Utils;
using ElsaMina.Logging;

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
            .GetAllNamedRegistrations<ICommand>()
            .DistinctBy(command => command.Name);
    }

    public async Task TryExecuteCommandAsync(string commandName, IContext context,
        CancellationToken cancellationToken = default)
    {
        if (_dependencyContainerService.IsRegisteredWithName<ICommand>(commandName))
        {
            Log.Information("Executing {0} as a normal command", commandName);
            var command = _dependencyContainerService.ResolveNamed<ICommand>(commandName);

            if (!CanCommandBeRan(context, command))
            {
                return;
            }

            await command.RunAsync(context, cancellationToken);
            return;
        }

        if (!context.IsPrivateMessage)
        {
            Log.Information("Trying command {0} as a custom command", commandName);
            if (await _addedCommandsManager.TryExecuteAddedCommand(commandName, context))
            {
                // Une commande custom a été trouvée & éxécutée : on s'arrête là
                return;
            }
        }

        Log.Error("Could not find command {0}", commandName);
        var canRunAutoCorrect = context.IsPrivateMessage
                                || (await context.Room.GetParameterValueAsync(Parameter.HasCommandAutoCorrect,
                                    cancellationToken)).ToBoolean();

        if (canRunAutoCorrect)
        {
            var maxLevenshteinDistance = commandName.Length <= 5 ? 1 : 2; // <= completement arbitraire
            var closestCommand = GetAllCommands()
                .SelectMany(command => (string[]) [.. command.Aliases, command.Name])
                .Where(possibleCommands => possibleCommands.LevenshteinDistance(commandName) <= maxLevenshteinDistance)
                .ToArray(); // TODO : ajouter les commandes custom un jour dans le cas d'une salle

            if (closestCommand.Length > 0)
            {
                Log.Information("Auto-correcting command {0} to {1}", commandName, closestCommand);
                context.ReplyLocalizedMessage("command_autocorrect_suggestion", commandName, string.Join(", ", closestCommand));
            }

            return;
        }

        Log.Information("Not running auto-correct for command {0}", commandName);
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

        if (!context.HasRankOrHigher(command.RequiredRank))
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