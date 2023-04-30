using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public interface ICommandExecutor
{
    Task TryExecuteCommand(string commandName, Context context);
}