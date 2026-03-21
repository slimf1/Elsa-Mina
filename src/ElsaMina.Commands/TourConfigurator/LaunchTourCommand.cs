using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.TourConfigurator;

[NamedCommand("launchtour", Aliases = ["lt"])]
public class LaunchTourCommand : Command
{
    private readonly IRoomsManager _roomsManager;
    private readonly ITourConfigService _tourConfigService;

    public LaunchTourCommand(IRoomsManager roomsManager, ITourConfigService tourConfigService)
    {
        _roomsManager = roomsManager;
        _tourConfigService = tourConfigService;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.Target))
        {
            context.ReplyLocalizedMessage("tourconfig_launchtour_usage");
            return;
        }

        var args = context.Target.Split(',');
        var tourId = args[0].Trim();
        var roomId = args.Length > 1 ? args[1].Trim() : context.RoomId;

        var room = _roomsManager.GetRoom(roomId);
        if (room == null)
        {
            context.ReplyLocalizedMessage("tourconfig_room_not_found");
            return;
        }

        if (!await context.HasSufficientRankInRoom(roomId, Rank.Driver, cancellationToken))
        {
            return;
        }

        var config = await _tourConfigService.GetTourConfigAsync(tourId, roomId, cancellationToken);
        if (config == null)
        {
            context.ReplyLocalizedMessage("tourconfig_not_found", tourId);
            return;
        }

        TourConfigLauncher.LaunchTournament(context, config);
    }
}
