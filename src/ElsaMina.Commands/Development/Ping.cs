using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Ping : BaseCommand<Ping>
{
    public static bool IsAllowedInPm => true;
    public static char RequiredRank => '+';
    public static string HelpMessageKey => "Returns pong.";

    public Ping()
    {
        Name = "ping";
        Aliases = new[] { "tdt" };
    }
    
    public override Task Run(IContext context)
    {
        context.Reply("pong");
        return Task.CompletedTask;
    }
}