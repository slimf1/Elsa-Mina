using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Ping : Command<Ping>, INamed
{
    public static string Name => "ping";
    public static List<string> Aliases => ["tdt"];

    public override bool IsAllowedInPm => true;
    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        context.Reply("pong");
        return Task.CompletedTask;
    }
}