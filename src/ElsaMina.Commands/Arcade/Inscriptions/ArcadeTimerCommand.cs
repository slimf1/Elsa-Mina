using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("arcadetimer", Aliases = ["timerarcade"])]
public class ArcadeTimerCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;

    public ArcadeTimerCommand(IArcadeInscriptionsManager inscriptionsManager)
    {
        _inscriptionsManager = inscriptionsManager;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "arcade_timer_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        if (!_inscriptionsManager.HasActiveInscriptions(context.RoomId))
        {
            context.ReplyLocalizedMessage("arcade_no_active_inscriptions");
            return Task.CompletedTask;
        }

        if (!int.TryParse(context.Target.Trim(), out var minutes))
        {
            context.ReplyLocalizedMessage("arcade_timer_parse_error");
            return Task.CompletedTask;
        }

        if (minutes <= 0)
        {
            context.ReplyLocalizedMessage("arcade_timer_positive_integer");
            return Task.CompletedTask;
        }

        _inscriptionsManager.StartTimer(context.RoomId, minutes);
        context.ReplyLocalizedMessage("arcade_timer_success", minutes);

        return Task.CompletedTask;
    }
}