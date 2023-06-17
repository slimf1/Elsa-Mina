using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Help : ICommand
{
    public static string Name => "help";
    public static IEnumerable<string> Aliases => new[] { "about" };
    public bool IsAllowedInPm => true;
    public char RequiredRank => '+';

    public Task Run(IContext context)
    {
        context.ReplyLocalizedMessage("help");
        return Task.CompletedTask;
    }
}