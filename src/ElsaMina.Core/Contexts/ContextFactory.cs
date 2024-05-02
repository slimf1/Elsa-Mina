using ElsaMina.Core.Models;

namespace ElsaMina.Core.Contexts;

public class ContextFactory : IContextFactory
{
    private readonly IContextProvider _contextProvider;

    public ContextFactory(IContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
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
            ContextType.Pm => new PmContext(_contextProvider, bot, message, target, sender, command),
            ContextType.Room => new RoomContext(_contextProvider, bot, message ,target, sender, command, room,
                timestamp),
            _ => throw new ArgumentException("Invalid type")
        };
    }
}