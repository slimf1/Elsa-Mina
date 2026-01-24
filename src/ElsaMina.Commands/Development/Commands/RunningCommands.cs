using System.Text;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Development.Commands;

[NamedCommand("running-commands", "running")]
public class RunningCommands : DevelopmentCommand
{
    private readonly ICommandExecutor _commandExecutor;

    public RunningCommands(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var runningCommands = _commandExecutor.RunningCommands;
        var stringBuilder = new StringBuilder();

        foreach (var (id, cmdName, ctx, _, _) in runningCommands)
        {
            stringBuilder.Append($"{cmdName} - id={id}, room={ctx.RoomId}, sender={ctx.Sender.UserId}<br />");
        }

        context.ReplyHtml(stringBuilder.ToString());

        return Task.CompletedTask;
    }
}