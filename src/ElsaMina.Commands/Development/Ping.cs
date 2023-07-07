using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Ping : BaseCommand<Ping>, INamed
{
    public static string Name => "ping";
    public static IEnumerable<string> Aliases => new[] { "tdt" };

    public override bool IsAllowedInPm => true;
    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        context.Reply("pong");
        return Task.CompletedTask;
    }
}