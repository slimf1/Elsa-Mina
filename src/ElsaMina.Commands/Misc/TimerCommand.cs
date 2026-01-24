using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc;

[NamedCommand("timer", "reminder")]
public class TimerCommand : Command
{
    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "timer_command_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var timeSpan = context.Target.ToTimeSpan();
        if (timeSpan == null)
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        var periodicTimer = new PeriodicTimer(timeSpan.Value);
        var startedBy = context.Sender.Name;
        context.ReplyRankAwareLocalizedMessage("timer_command_started");
        await periodicTimer.WaitForNextTickAsync(cancellationToken);
        context.ReplyRankAwareLocalizedMessage("timer_command_success", startedBy);
    }
}