using ElsaMina.Core.Contexts;
using ElsaMina.Core.Utils;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Showdown.BattleTracker;

[NamedCommand("starttracking", "battletracking", "starttracking", "track", "start-tracking",
    "stoptracking", "stoptrack", "stop-tracking", "stop-track", "endtrack", "end-tracking", "endtracking")]
public class ToggleLadderTrackerCommand : Command
{
    private readonly ILadderTrackerManager _ladderTrackerManager;

    public ToggleLadderTrackerCommand(ILadderTrackerManager ladderTrackerManager)
    {
        _ladderTrackerManager = ladderTrackerManager;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override string HelpMessageKey => "toggletracking_help_message";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var roomId = context.RoomId;
        var parts = context.Target?.Split(',', 2);

        if (string.IsNullOrWhiteSpace(roomId) || parts == null || parts.Length != 2)
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        var format = parts[0].ToLowerAlphaNum();
        var prefix = parts[1].ToLowerAlphaNum();

        if (string.IsNullOrWhiteSpace(format) || string.IsNullOrWhiteSpace(prefix))
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        var shouldEnd = context.Command is "stoptracking" or "stoptrack" 
            or "stop-tracking" or "stop-track" or "endtrack"
            or "end-tracking" or "endtracking";
        try
        {
            if (shouldEnd)
            {
                _ladderTrackerManager.StopTracking(roomId, format, prefix);
                context.ReplyLocalizedMessage("toggletracking_stop_success", format, prefix);
            }
            else
            {
                _ladderTrackerManager.StartTracking(roomId, format, prefix);
                context.ReplyLocalizedMessage("toggletracking_success", format, prefix);
            }

        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while starting battle tracking for room {RoomId}", roomId);
            context.ReplyLocalizedMessage("toggletracking_failure");
        }

        return Task.CompletedTask;
    }
}
