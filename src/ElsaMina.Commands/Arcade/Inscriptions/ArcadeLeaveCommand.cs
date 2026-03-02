using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("leave", Aliases = ["out"])]
public class ArcadeLeaveCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;
    private readonly IRoomsManager _roomsManager;

    public ArcadeLeaveCommand(IArcadeInscriptionsManager inscriptionsManager, IRoomsManager roomsManager)
    {
        _inscriptionsManager = inscriptionsManager;
        _roomsManager = roomsManager;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override bool IsAllowedInPrivateMessage => true;
    public override string HelpMessageKey => "arcade_leave_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string roomId;

        if (context.IsPrivateMessage)
        {
            if (string.IsNullOrWhiteSpace(context.Target))
            {
                context.Reply(context.GetString("arcade_leave_pm_specify_room"));
                return Task.CompletedTask;
            }

            roomId = context.Target.ToLowerAlphaNum();
            if (!_roomsManager.HasRoom(roomId))
            {
                context.Reply(context.GetString("arcade_pm_room_not_found"));
                return Task.CompletedTask;
            }
        }
        else
        {
            roomId = context.RoomId;
        }

        if (!_inscriptionsManager.HasActiveInscriptions(roomId))
        {
            context.Reply(context.GetString("arcade_no_active_inscriptions"));
            return Task.CompletedTask;
        }

        _inscriptionsManager.TryGetState(roomId, out var state);
        var userId = context.Sender.UserId;

        if (!state.Participants.Contains(userId))
        {
            context.Reply(context.GetString("arcade_leave_not_registered"));
            return Task.CompletedTask;
        }

        state.Participants.Remove(userId);
        context.Reply(context.GetString("arcade_leave_success", state.Participants.Count));

        return Task.CompletedTask;
    }
}