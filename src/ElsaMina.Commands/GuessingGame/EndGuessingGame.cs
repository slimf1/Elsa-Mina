using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;

namespace ElsaMina.Commands.GuessingGame;

[NamedCommand("endguessinggame", Aliases = ["endcountriesgame"])]
public class EndGuessingGame : Command
{
    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (context.Room?.Game is IGuessingGame guessingGame)
        {
            guessingGame.Cancel();
            context.ReplyLocalizedMessage("end_guessing_game_success");
            return Task.CompletedTask;
        }

        context.ReplyLocalizedMessage("end_guessing_game_no_game");
        return Task.CompletedTask;
    }
}