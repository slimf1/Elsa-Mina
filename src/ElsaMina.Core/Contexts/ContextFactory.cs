using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public class ContextFactory : IContextFactory
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;

    public ContextFactory(IConfigurationManager configurationManager,
        IResourcesService resourcesService)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
    }

    public IContext GetContext(ContextType type,
        IBot bot,
        string message,
        string target,
        IUser sender,
        string command,
        IRoom room = null,
        long timestamp = 0)
    {
        return type switch
        {
            ContextType.Pm => new PmContext(_configurationManager, _resourcesService, bot, message, target, sender,
                command),
            ContextType.Room => new RoomContext(_configurationManager, _resourcesService, bot, message ,target, sender,
                command, room, timestamp),
            _ => throw new ArgumentException("Invalid type")
        };
    }
}