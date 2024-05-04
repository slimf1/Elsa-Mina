using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

[NamedCommand("help", Aliases = ["about"])]
public class Help : Command
{
    public override bool IsAllowedInPm => true;
    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        context.ReplyLocalizedMessage("help");
        return Task.CompletedTask;
    }
}