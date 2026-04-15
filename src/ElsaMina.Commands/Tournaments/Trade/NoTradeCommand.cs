using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Tournaments.Trade;

[NamedCommand("notrade")]
public class NoTradeCommand : Command
{
    private const string STAFF_ROOM = "frenchstaff";

    public override Rank RequiredRank => Rank.Driver;
    public override bool IsAllowedInPrivateMessage => true;
    public override string HelpMessageKey => "notrade_help";

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        var parts = context.Target.Split(',');
        if (parts.Length < 2)
        {
            ReplyLocalizedHelpMessage(context);
            return Task.CompletedTask;
        }

        var originUsername = parts[0].Trim();
        var points = parts[1].Trim();

        context.SendMessageIn(STAFF_ROOM,
            $"/adduhtml trade-req-{originUsername}-{points}, " +
            context.GetString("notrade_staff_refused", originUsername));

        return Task.CompletedTask;
    }
}
