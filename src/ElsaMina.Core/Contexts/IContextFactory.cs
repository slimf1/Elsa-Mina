using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Contexts;

public interface IContextFactory
{
    IContext GetContext(ContextType type, IBot bot, string target, IUser sender, string command, IRoom room = null,
        long timestamp = 0);
}