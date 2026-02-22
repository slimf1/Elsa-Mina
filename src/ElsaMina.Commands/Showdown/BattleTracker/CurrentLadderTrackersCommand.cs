using System.Text;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Showdown.BattleTracker;

[NamedCommand("running-trackers", "currenttrackers", "trackers", "currenttrack", "laddertrack", "laddertrackers",
    "ladder-trackers")]
public class CurrentLadderTrackersCommand : Command
{
    private readonly ILadderTrackerManager _ladderTrackerManager;
    private readonly IFormatsManager _formatsManager;

    public CurrentLadderTrackersCommand(ILadderTrackerManager ladderTrackerManager, IFormatsManager formatsManager)
    {
        _ladderTrackerManager = ladderTrackerManager;
        _formatsManager = formatsManager;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var roomTrackings = _ladderTrackerManager.GetRoomTrackings(context.RoomId);
        if (roomTrackings.Count == 0)
        {
            context.ReplyLocalizedMessage("current_ladder_trackers_none", context.RoomId);
            return Task.CompletedTask;
        }

        var stringBuilder = new StringBuilder();
        foreach (var tracking in roomTrackings)
        {
            stringBuilder.AppendLine(context.GetString(
                "current_ladder_trackers_entry",
                _formatsManager.GetCleanFormat(tracking.Format),
                tracking.Prefix));
        }

        context.ReplyHtml(stringBuilder.ToString());
        return Task.CompletedTask;
    }
}