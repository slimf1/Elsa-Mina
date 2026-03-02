using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Arcade.Inscriptions;

[NamedCommand("arcaderemove", Aliases = ["removearcade"])]
public class ArcadeRemoveCommand : Command
{
    private readonly IArcadeInscriptionsManager _inscriptionsManager;

    public ArcadeRemoveCommand(IArcadeInscriptionsManager inscriptionsManager)
    {
        _inscriptionsManager = inscriptionsManager;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "arcade_remove_help";

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

        if (!state.Participants.Contains(targetUserId))
        {
            context.ReplyLocalizedMessage("arcade_remove_not_registered", context.Target);
            return Task.CompletedTask;
        }

        state.Participants.Remove(targetUserId);
        state.BannedUsers.Add(targetUserId);
        context.ReplyLocalizedMessage("arcade_remove_success", context.Target);

        var remainingCount = state.Participants.Count;

        if (remainingCount == 1)
        {
            var winnerId = state.Participants.First();
            var winnerName = context.Room.Users.TryGetValue(winnerId, out IUser winnerUser) ? winnerUser.Name : winnerId;
            var html = $"<b>🏆 {state.Title} - Tournoi terminé !</b><br><b>Gagnant :</b> {winnerName}";
            context.Reply($"/addhtmlbox {html}");
            state.IsActive = false;
        }
        else if (remainingCount == 0)
        {
            var html = $"<b>⏰ {state.Title} - Tournoi terminé</b><br>Aucun participant restant.";
            context.Reply($"/addhtmlbox {html}");
            state.IsActive = false;
        }
        else
        {
            var participantNames = state.Participants.Select(userId =>
            {
                if (context.Room.Users.TryGetValue(userId, out IUser user))
                {
                    return user.Name;
                }
                return userId;
            }).ToList();

            var html = $"<b>📋 {state.Title} - Mise à jour</b><br><b>Participants restants ({remainingCount}) :</b> {string.Join(", ", participantNames)}";
            context.Reply($"/addhtmlbox {html}");
        }

        return Task.CompletedTask;
    }
}