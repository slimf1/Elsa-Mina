using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public interface ICommandExecutor
{
    bool HasCommand(string commandName);
    Task TryExecuteCommand(string commandName, IContext context);
}