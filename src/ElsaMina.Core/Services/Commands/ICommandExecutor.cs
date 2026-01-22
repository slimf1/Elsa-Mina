using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public interface ICommandExecutor
{
    IEnumerable<ICommand> GetAllCommands();
    Task TryExecuteCommandAsync(string commandName, IContext context, CancellationToken cancellationToken = default);
}