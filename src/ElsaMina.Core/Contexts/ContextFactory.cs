using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Contexts;

public class ContextFactory : IContextFactory
{
    private readonly IContextProvider _contextProvider;
    private readonly IBot _bot;
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;
    private readonly IPmSendersManager _pmSendersManager;

    public ContextFactory(IContextProvider contextProvider,
        IBot bot,
        IRoomsManager roomsManager,
        IConfigurationManager configurationManager,
        IPmSendersManager pmSendersManager)
    {
        _contextProvider = contextProvider;
        _bot = bot;
        _roomsManager = roomsManager;
        _configurationManager = configurationManager;
        _pmSendersManager = pmSendersManager;
    }

    public IContext TryBuildContextFromReceivedMessage(string[] parts, string roomId = null)
    {
        switch (parts.Length)
        {
            case > 1 when parts[1] == "c:":
            {
                var room = _roomsManager.GetRoom(roomId);
                var timestamp = long.Parse(parts[2]);
                var userId = parts[3].ToLowerAlphaNum();
                var message = parts[4];
                var (target, command) = GetTargetAndCommand(message);
                return new RoomContext(_contextProvider, _bot, message, target,
                    room.Users[userId], command, room, timestamp);
            }
            case > 2 when parts[1] == "pm":
            {
                var message = parts[4];
                var (target, command) = GetTargetAndCommand(message);
                return new PmContext(_contextProvider, _bot, message, target, _pmSendersManager.GetUser(parts[2]), command);
            }
            default:
                return null;
        }
    }

    private (string, string) GetTargetAndCommand(string message)
    {
        var trigger = _configurationManager.Configuration.Trigger;
        return message.StartsWith(trigger)
            ? Parsing.ParseMessage(message, trigger)
            : (null, null);
    }
}