using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Contexts;

public class ContextFactory : IContextFactory
{
    private readonly IConfigurationService _configurationService;

    public ContextFactory(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public Context GetContext(ContextType type,
        IBot bot,
        string target,
        IUser sender,
        string command,
        IRoom? room = null,
        long? timestamp = null)
    {
        return type switch
        {
            ContextType.Pm => new PmContext(_configurationService, bot, target, sender, command),
            ContextType.Room => new RoomContext(_configurationService, bot, target, sender, command, room!,
                timestamp ?? 0),
            _ => throw new ArgumentException("Invalid type")
        };
    }
}