using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

[NamedCommand("ping", Aliases = ["tdt"])]
public class Ping : Command
{
    public override bool IsAllowedInPrivateMessage => true;
    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        context.Reply("pong");
        return Task.CompletedTask;
    }
}