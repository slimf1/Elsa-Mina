using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Tournaments.Betting;

[NamedCommand("cancelbet", Aliases = ["cancel-bet"])]
public class CancelBetCommand : Command
{
    private readonly ITournamentBettingService _tournamentBettingService;

    public CancelBetCommand(ITournamentBettingService tournamentBettingService)
    {
        _tournamentBettingService = tournamentBettingService;
    }

    public override Rank RequiredRank => Rank.Regular;
    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var parts = context.Target.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            context.ReplyLocalizedMessage("cancelbet_help");
            return;
        }

        var roomId = parts[0].ToLowerAlphaNum();
        var targetPlayer = parts.Length >= 2 ? parts[1].ToLowerAlphaNum() : null;

        var bettorId = context.Sender.UserId.ToLowerAlphaNum();
        var cancelled = await _tournamentBettingService.CancelBetAsync(bettorId, roomId, targetPlayer, cancellationToken);

        if (cancelled > 0)
        {
            context.ReplyLocalizedMessage(targetPlayer != null ? "cancelbet_success" : "cancelbet_all_success",
                targetPlayer ?? roomId);
        }
        else
        {
            context.ReplyLocalizedMessage("cancelbet_not_found", targetPlayer ?? roomId);
        }
    }
}