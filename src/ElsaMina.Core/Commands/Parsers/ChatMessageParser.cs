using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class ChatMessageParser : Parser
{
    private readonly IContextFactory _contextFactory;
    private readonly IBot _bot;
    private readonly IRoomsManager _roomsManager;
    private readonly IConfigurationManager _configurationManager;

    protected ChatMessageParser(IDependencyContainerService dependencyContainerService)
    {
        _contextFactory = dependencyContainerService.Resolve<IContextFactory>();
        _bot = dependencyContainerService.Resolve<IBot>();
        _roomsManager = dependencyContainerService.Resolve<IRoomsManager>();
        _configurationManager = dependencyContainerService.Resolve<IConfigurationManager>();
    }

    public sealed override async Task Execute(string[] parts, string roomId = null)
    {
        if (parts.Length > 1 && parts[1] == "c:")
        {
            var timestamp = long.Parse(parts[2]);
            var room = _roomsManager.GetRoom(roomId);
            var userId = parts[3].ToLowerAlphaNum();
            string target = null;
            string command = null;
            if (parts[4].StartsWith(_configurationManager.Configuration.Trigger))
            {
                (target, command) = Parsing.ParseMessage(parts[4], _configurationManager.Configuration.Trigger);
            }
            var context = _contextFactory.GetContext(ContextType.Room, _bot, parts[4], target,
                room.Users[userId], command, room, timestamp);
            await HandleChatMessage(context);
        }
    }

    protected abstract Task HandleChatMessage(IContext context);
}