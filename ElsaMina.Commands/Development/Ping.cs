using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Ping : ICommand
{
    public static string Name => "ping";
    public static IEnumerable<string> Aliases => new[] { "tdt" };

    public static bool IsAllowedInPm => true;
    public static char RequiredRank => '+';
    public static string HelpMessageKey => "Returns pong.";

    public Task Run(IContext context)
    {
        context.Reply("pong");
        return Task.CompletedTask;
    }
}