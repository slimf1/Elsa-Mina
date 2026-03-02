using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("arcadestop", Aliases = ["stoparcade"])]
public class ArcadeStopCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;

    public ArcadeStopCommand(IArcadeInscriptionsManager inscriptionsManager)
    {
        _inscriptionsManager = inscriptionsManager;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "arcade_stop_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (!_inscriptionsManager.HasActiveInscriptions(context.RoomId))
        {
            context.ReplyLocalizedMessage("arcade_no_active_inscriptions");
            return Task.CompletedTask;
        }

        _inscriptionsManager.TryGetState(context.RoomId, out var state);

        var participantNames = state.Participants.Select(userId =>
        {
            if (context.Room.Users.TryGetValue(userId, out IUser user))
            {
                return user.Name;
            }
            return userId;
        }).ToList();

        _inscriptionsManager.StopInscriptions(context.RoomId);

        if (participantNames.Count > 0)
        {
            var html = $"<b>🛑 {state.Title} - Arrêté manuellement</b><br><b>Participants finaux ({participantNames.Count}) :</b> {string.Join(", ", participantNames)}";
            context.Reply($"/addhtmlbox {html}");
        }
        else
        {
            context.ReplyLocalizedMessage("arcade_stop_no_participants");
        }

        return Task.CompletedTask;
    }
}