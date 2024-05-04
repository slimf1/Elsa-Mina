using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Repeats;

[NamedCommand("stop-repeat", Aliases = ["end-repeat", "cancel-repeat", "stoprepeat", "endrepeat"])]
public class StopRepeat : Command
{
    private readonly IRepeatsManager _repeatsManager;

    public StopRepeat(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override char RequiredRank => '%';

    public override Task Run(IContext context)
    {
        var ended = _repeatsManager.StopRepeat(context.RoomId, context.Target.ToLowerAlphaNum());
        if (!ended)
        {
            context.Reply("Le repeat n'a pas pu être terminé.");
            return Task.CompletedTask;
        }

        context.Reply("Le repeat a été terminé.");
        return Task.CompletedTask;
    }
}