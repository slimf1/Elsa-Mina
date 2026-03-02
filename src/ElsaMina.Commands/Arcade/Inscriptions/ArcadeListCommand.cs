using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("arcadelist", Aliases = ["listarcade"])]
public class ArcadeListCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;

    public ArcadeListCommand(IArcadeInscriptionsManager inscriptionsManager)
    {
        _inscriptionsManager = inscriptionsManager;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override string HelpMessageKey => "arcade_list_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (!_inscriptionsManager.TryGetState(context.RoomId, out var state))
        {
            context.ReplyLocalizedMessage("arcade_list_no_data");
            return Task.CompletedTask;
        }

        if (state.Participants.Count == 0)
        {
            var status = context.GetString(state.IsActive ? "arcade_list_status_active" : "arcade_list_status_inactive");
            context.ReplyLocalizedMessage("arcade_list_no_participants", status);
            return Task.CompletedTask;
        }

        var participantNames = state.Participants.Select(userId =>
        {
            if (context.Room.Users.TryGetValue(userId, out IUser user))
            {
                return user.Name;
            }
            return userId;
        }).ToList();

        var statusString = context.GetString(state.IsActive ? "arcade_list_status_active" : "arcade_list_status_inactive");
        var timerInfo = "";

        if (state.IsActive && state.TimerEnd.HasValue)
        {
            var remaining = state.TimerEnd.Value - DateTimeOffset.UtcNow;
            if (remaining.TotalSeconds > 0)
            {
                timerInfo = context.GetString("arcade_list_timer_remaining", (int)remaining.TotalMinutes);
            }
        }

        context.ReplyLocalizedMessage("arcade_list_result",
            participantNames.Count,
            statusString,
            timerInfo,
            string.Join(", ", participantNames));

        return Task.CompletedTask;
    }
}