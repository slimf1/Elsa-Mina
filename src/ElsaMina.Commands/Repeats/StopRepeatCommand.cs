using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Repeats;

[NamedCommand("stop-repeat", Aliases = ["end-repeat", "cancel-repeat", "stoprepeat", "endrepeat"])]
public class StopRepeatCommand : Command
{
    private readonly IRepeatsManager _repeatsManager;

    public StopRepeatCommand(IRepeatsManager repeatsManager)
    {
        _repeatsManager = repeatsManager;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(context.Target, out var guid))
        {
            context.Reply("ID invalide");
            return Task.CompletedTask;
        }

        var ended = _repeatsManager.StopRepeat(context.RoomId, guid);
        if (!ended)
        {
            context.Reply("Le repeat n'a pas pu être terminé.");
            return Task.CompletedTask;
        }

        context.Reply("Le repeat a été terminé.");
        return Task.CompletedTask;
    }
}