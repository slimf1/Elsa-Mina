using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Ping : ICommand
{
    public static string Name => "ping";
    public static IEnumerable<string> Aliases => new[] { "tdt" };

    public Task Run(Context context)
    {
        context.Reply("ping");
        return Task.CompletedTask;
    }
}