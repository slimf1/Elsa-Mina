using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("end-connect-four", Aliases = ["c4end", "c4-end"])]
public class EndConnectFour : Command
{
    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (context.Room?.Game is IConnectFourGame connectFourGame)
        {
            connectFourGame.Cancel();
            context.ReplyLocalizedMessage("c4_game_cancelled");
        }
        else
        {
            context.ReplyLocalizedMessage("c4_game_ongoing_game");
        }
        
        return Task.CompletedTask;
    }
}