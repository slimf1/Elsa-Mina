using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("in", Aliases = ["join"])]
public class ArcadeInCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;
    private readonly IRoomsManager _roomsManager;

    public ArcadeInCommand(IArcadeInscriptionsManager inscriptionsManager, IRoomsManager roomsManager)
    {
        _inscriptionsManager = inscriptionsManager;
        _roomsManager = roomsManager;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override bool IsAllowedInPrivateMessage => true;
    public override string HelpMessageKey => "arcade_in_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string roomId;

        if (context.IsPrivateMessage)
        {
            if (string.IsNullOrWhiteSpace(context.Target))
            {
                context.ReplyLocalizedMessage("arcade_in_pm_specify_room");
                return Task.CompletedTask;
            }

            roomId = context.Target.ToLowerAlphaNum();
            if (!_roomsManager.HasRoom(roomId))
            {
                context.ReplyLocalizedMessage("arcade_pm_room_not_found");
                return Task.CompletedTask;
            }
        }
        else
        {
            roomId = context.RoomId;
        }

        if (!_inscriptionsManager.HasActiveInscriptions(roomId))
        {
            context.ReplyLocalizedMessage("arcade_no_active_inscriptions");
            return Task.CompletedTask;
        }

        _inscriptionsManager.TryGetState(roomId, out var state);

        if (state.IsTimerExpired)
        {
            context.ReplyLocalizedMessage("arcade_inscriptions_timer_expired");
            return Task.CompletedTask;
        }

        var userId = context.Sender.UserId;

        if (state.BannedUsers.Contains(userId))
        {
            context.ReplyLocalizedMessage("arcade_in_banned");
            return Task.CompletedTask;
        }

        if (state.Participants.Contains(userId))
        {
            context.ReplyLocalizedMessage("arcade_in_already_registered");
            return Task.CompletedTask;
        }

        state.Participants.Add(userId);
        context.ReplyLocalizedMessage("arcade_in_success", state.Participants.Count);

        if (context.IsPrivateMessage)
        {
            context.SendMessageIn(roomId, $"/modnote {context.Sender.Name} vient de s'inscrire aux inscriptions du tournoi arcade (via MP)");
        }
        else
        {
            context.SendMessageIn(roomId, $"/modnote {context.Sender.Name} vient de s'inscrire aux inscriptions du tournoi arcade");
        }

        return Task.CompletedTask;
    }
}