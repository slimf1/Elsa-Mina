using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Ping : ICommand
{
    public static string Name => "ping";
    public static IEnumerable<string> Aliases => new[] { "tdt" };

    public static bool IsAllowedInPm => true;
    public static char RequiredRank => '+';
    public static string HelpMessage => "Returns pong.";

    public Task Run(Context context)
    {
        context.Reply("pong");
        return Task.CompletedTask;
    }
}