using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Development.Commands;

[NamedCommand("cancel-command", "cancelcmd")]
public class CancelRunningCommand : DevelopmentCommand
{
    private readonly ICommandExecutor _commandExecutor;

    public CancelRunningCommand(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var target = context.Target?.Trim();
        if (string.IsNullOrWhiteSpace(target) || !Guid.TryParse(target, out var executionId))
        {
            context.ReplyLocalizedMessage("cancel_command_usage");
            return Task.CompletedTask;
        }

        var cancelled = _commandExecutor.TryCancel(executionId);
        context.ReplyLocalizedMessage(cancelled
            ? "cancel_command_success"
            : "cancel_command_not_found", executionId);

        return Task.CompletedTask;
    }
}
