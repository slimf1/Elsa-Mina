using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Commands;

public interface ICommandExecutor
{
    IReadOnlyCollection<RunningCommand> RunningCommands { get; }
    IEnumerable<ICommand> GetAllCommands();
    Task TryExecuteCommandAsync(string commandName, IContext context, CancellationToken cancellationToken = default);
    bool TryCancel(Guid executionId);
}