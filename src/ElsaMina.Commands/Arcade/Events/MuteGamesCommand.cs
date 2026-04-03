using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Arcade.Events;

[NamedCommand("mutegames", Aliases = ["disablegames"])]
public class MuteGamesCommand : Command
{
    private const int DEFAULT_DURATION_MINUTES = 30;

    private readonly IArcadeEventsService _arcadeEventsService;

    public MuteGamesCommand(IArcadeEventsService arcadeEventsService)
    {
        _arcadeEventsService = arcadeEventsService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var minutes = DEFAULT_DURATION_MINUTES;
        if (!string.IsNullOrWhiteSpace(context.Target) && int.TryParse(context.Target.Trim(), out var parsed) && parsed > 0)
        {
            minutes = parsed;
        }

        _arcadeEventsService.MuteGames(context.RoomId, TimeSpan.FromMinutes(minutes));
        context.ReplyLocalizedMessage("games_muted", minutes);
        return Task.CompletedTask;
    }
}
