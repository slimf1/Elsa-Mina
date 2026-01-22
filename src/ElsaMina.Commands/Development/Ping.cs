using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Development;

[NamedCommand("ping", Aliases = ["tdt"])]
public class Ping : Command
{
    public override bool IsAllowedInPrivateMessage => true;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        context.Reply("pong", rankAware: true);
        return Task.CompletedTask;
    }
}