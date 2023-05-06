using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Contexts;

public class ContextFactory : IContextFactory
{
    private readonly IConfigurationManager _configurationManager;

    public ContextFactory(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public Context GetContext(ContextType type,
        IBot bot,
        string target,
        IUser sender,
        string command,
        IRoom room = null,
        long timestamp = 0)
    {
        return type switch
        {
            ContextType.Pm => new PmContext(_configurationManager, bot, target, sender, command),
            ContextType.Room => new RoomContext(_configurationManager, bot, target, sender, command, room,
                timestamp),
            _ => throw new ArgumentException("Invalid type")
        };
    }
}