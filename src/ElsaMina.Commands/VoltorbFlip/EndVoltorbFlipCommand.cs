using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.VoltorbFlip;

[NamedCommand("vfend", Aliases = ["end-voltorbflip"])]
public class EndVoltorbFlipCommand : Command
{
    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (context.Room?.Game is IVoltorbFlipGame voltorbFlip)
        {
            if (context.Sender.UserId != voltorbFlip.Owner.UserId)
            {
                context.ReplyLocalizedMessage("vf_game_not_owner");
                return Task.CompletedTask;
            }

            voltorbFlip.Cancel();
            context.ReplyLocalizedMessage("vf_game_cancelled");
        }
        else
        {
            context.ReplyLocalizedMessage("vf_game_no_game");
        }

        return Task.CompletedTask;
    }
}
