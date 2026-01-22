using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;
using Timer = System.Timers.Timer;

namespace ElsaMina.Commands.Misc;

[NamedCommand("timer", "reminder")]
public class TimerCommand : Command
{
    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "timer_command_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var timeSpan = context.Target.ToTimeSpan();
        if (timeSpan == null)
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        var startedBy = context.Sender.Name;
        var timer = new Timer();
        timer.Interval = timeSpan.Value.TotalMilliseconds;
        timer.AutoReset = false;
        timer.Enabled = true;
        timer.Elapsed += (_, _) =>
        {
            context.ReplyRankAwareLocalizedMessage("timer_command_success", startedBy);
            
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();

        context.ReplyRankAwareLocalizedMessage("timer_command_started");
        return Task.CompletedTask;
    }
}