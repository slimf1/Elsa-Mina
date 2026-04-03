using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Arcade.Events;

[NamedCommand("unmutegames", Aliases = ["enablegames"])]
public class UnmuteGamesCommand : Command
{
    private readonly IArcadeEventsService _arcadeEventsService;

    public UnmuteGamesCommand(IArcadeEventsService arcadeEventsService)
    {
        _arcadeEventsService = arcadeEventsService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        _arcadeEventsService.UnmuteGames(context.RoomId);
        context.ReplyLocalizedMessage("games_unmuted");
        return Task.CompletedTask;
    }
}
