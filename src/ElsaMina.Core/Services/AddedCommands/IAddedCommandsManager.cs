using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.AddedCommands;

public interface IAddedCommandsManager
{
    Task TryExecuteAddedCommand(string commandName, IContext context);
}