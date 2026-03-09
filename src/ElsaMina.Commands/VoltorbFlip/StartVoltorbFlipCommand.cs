using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.VoltorbFlip;

[NamedCommand("voltorbflip", Aliases = ["vf", "voltorb-flip"])]
public class StartVoltorbFlipCommand : Command
{
    private readonly IDependencyContainerService _dependencyContainerService;

    public StartVoltorbFlipCommand(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var room = context.Room;

        if (room.Game is IVoltorbFlipGame voltorbFlip)
        {
            if (voltorbFlip.IsRoundActive)
            {
                context.ReplyLocalizedMessage("vf_game_round_active");
                return;
            }

            voltorbFlip.Context = context;
            await voltorbFlip.StartNewRound();
            return;
        }

        if (room.Game != null)
        {
            context.ReplyLocalizedMessage("vf_game_already_running");
            return;
        }

        var game = _dependencyContainerService.Resolve<VoltorbFlipGame>();
        game.Context = context;
        game.Owner = context.Sender;
        room.Game = game;
        await game.StartNewRound();
    }
}
