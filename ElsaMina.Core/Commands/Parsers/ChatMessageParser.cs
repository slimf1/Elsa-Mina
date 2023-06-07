using ElsaMina.Core.Bot;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class ChatMessageParser : Parser
{
    private readonly IContextFactory _contextFactory;
    private readonly IBot _bot;
    private readonly IRoomsManager _roomsManager;

    protected ChatMessageParser(IContextFactory contextFactory,
        IBot bot,
        IRoomsManager roomsManager)
    {
        _contextFactory = contextFactory;
        _bot = bot;
        _roomsManager = roomsManager;
    }

    public sealed override async Task Execute(string[] parts, string roomId = null)
    {
        if (parts.Length > 1 && parts[1] == "c:")
        {
            var timestamp = long.Parse(parts[2]);
            var room = _roomsManager.GetRoom(roomId);
            var userId = parts[3].ToLowerAlphaNum();
            var context = _contextFactory.GetContext(ContextType.Room, _bot, parts[4], room.Users[userId],
                null, room, timestamp);
            await HandleChatMessage(context);
        }
    }

    protected abstract Task HandleChatMessage(IContext context);
}