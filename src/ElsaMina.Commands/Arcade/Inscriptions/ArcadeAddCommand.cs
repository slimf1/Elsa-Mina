using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("arcadeadd", Aliases = ["addarcade"])]
public class ArcadeAddCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;

    public ArcadeAddCommand(IArcadeInscriptionsManager inscriptionsManager)
    {
        _inscriptionsManager = inscriptionsManager;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "arcade_add_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        if (!_inscriptionsManager.TryGetState(context.RoomId, out var state))
        {
            context.ReplyLocalizedMessage("arcade_list_no_data");
            return Task.CompletedTask;
        }

        var targetUserId = context.Target.ToLowerAlphaNum();

        if (state.Participants.Contains(targetUserId))
        {
            context.ReplyLocalizedMessage("arcade_add_already_registered", context.Target);
            return Task.CompletedTask;
        }

        if (state.BannedUsers.Contains(targetUserId))
        {
            context.ReplyLocalizedMessage("arcade_add_banned", context.Target);
            return Task.CompletedTask;
        }

        state.Participants.Add(targetUserId);
        context.ReplyLocalizedMessage("arcade_add_success", context.Target);

        context.SendMessageIn(context.RoomId,
            $"/modnote {context.Target} a été ajouté aux inscriptions du tournoi arcade par {context.Sender.Name}");

        var participantNames = state.Participants.Select(userId =>
        {
            if (context.Room.Users.TryGetValue(userId, out IUser user))
            {
                return user.Name;
            }
            return userId;
        }).ToList();

        var html = $"<b>➕ {state.Title} - Participant ajouté</b><br><b>Participants actuels ({state.Participants.Count}) :</b> {string.Join(", ", participantNames)}";
        context.Reply($"/addhtmlbox {html}");

        return Task.CompletedTask;
    }
}