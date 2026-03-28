using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.LightsOut;

[NamedCommand("lightsout", Aliases = ["lo"])]
public class StartLightsOutCommand : Command
{
    private readonly IDependencyContainerService _dependencyContainerService;
    private readonly IRoomsManager _roomsManager;
    private readonly ILightsOutGameManager _gameManager;

    public StartLightsOutCommand(IDependencyContainerService dependencyContainerService,
        IRoomsManager roomsManager,
        ILightsOutGameManager gameManager)
    {
        _dependencyContainerService = dependencyContainerService;
        _roomsManager = roomsManager;
        _gameManager = gameManager;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (context.IsPrivateMessage)
        {
            await HandlePrivateMessageAsync(context);
            return;
        }

        await HandleRoomMessageAsync(context);
    }

    private async Task HandlePrivateMessageAsync(IContext context)
    {
        var roomId = context.Target?.Trim();
        if (string.IsNullOrEmpty(roomId))
        {
            context.ReplyLocalizedMessage("lo_pm_missing_room");
            return;
        }

        var room = _roomsManager.GetRoom(roomId);
        if (room == null)
        {
            context.ReplyLocalizedMessage("lo_pm_invalid_room");
            return;
        }

        context.Culture = room.Culture;

        var userId = context.Sender.UserId;
        var existingGame = _gameManager.GetGame(roomId, userId);
        if (existingGame != null)
        {
            if (existingGame.IsRoundActive)
            {
                context.ReplyLocalizedMessage("lo_game_round_active");
                return;
            }

            existingGame.Context = context;
            await existingGame.StartNewRound();
            return;
        }

        var game = _dependencyContainerService.Resolve<LightsOutGame>();
        game.Context = context;
        game.Owner = context.Sender;
        game.IsPrivateMode = true;
        game.TargetRoomId = roomId;
        game.TargetUserId = userId;
        _gameManager.RegisterGame(roomId, userId, game);
        await game.StartNewRound();
    }

    private async Task HandleRoomMessageAsync(IContext context)
    {
        var room = context.Room;

        if (room.Game is ILightsOutGame existingGame)
        {
            if (existingGame.IsRoundActive)
            {
                context.ReplyLocalizedMessage("lo_game_round_active");
                return;
            }

            existingGame.Owner = context.Sender;
            existingGame.Context = context;
            await existingGame.StartNewRound();
            return;
        }

        if (room.Game != null)
        {
            context.ReplyLocalizedMessage("lo_game_already_running");
            return;
        }

        var game = _dependencyContainerService.Resolve<LightsOutGame>();
        game.Context = context;
        game.Owner = context.Sender;
        room.Game = game;
        await game.StartNewRound();
    }
}
