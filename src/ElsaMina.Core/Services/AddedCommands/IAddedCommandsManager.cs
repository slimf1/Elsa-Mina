using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.AddedCommands;

public interface IAddedCommandsManager
{
    Task<bool> TryExecuteAddedCommand(string commandName, IContext context);
}