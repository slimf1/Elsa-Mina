using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.RoomDashboard;

[NamedCommand("room-config", Aliases = ["roomconfig", "rc"])]
public class RoomConfig : Command
{
    private readonly IRoomsManager _roomsManager;

    public RoomConfig(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override bool IsWhitelistOnly => true; // todo : only authed used from room
    public override bool IsPrivateMessageOnly => true;

    // TODO à revoir
    public override async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        var roomId = parts[0].Trim().ToLower();

        var room = _roomsManager.GetRoom(roomId);
        if (room == null)
        {
            context.ReplyLocalizedMessage("room_config_room_not_found", roomId);
            return;
        }

        try
        {
            foreach (var pair in parts.Skip(1))
            {
                var items = pair.Split('=');
                var parameterId = items[0];
                var value = items[1];
                await _roomsManager.SetRoomBotConfigurationParameterValue(roomId, parameterId, value);
            }

            context.ReplyLocalizedMessage("room_config_success", roomId);
        }
        catch (Exception exception)
        {
            Logger.Current.Error(exception, "An error occurred while updating room configuration");
            context.ReplyLocalizedMessage("room_config_failure", exception.Message);
        }
    }
}