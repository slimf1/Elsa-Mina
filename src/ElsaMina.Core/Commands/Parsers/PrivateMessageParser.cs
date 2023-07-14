using ElsaMina.Core.Bot;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.PrivateMessages;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class PrivateMessageParser : Parser
{
    private readonly IContextFactory _contextFactory;
    private readonly IBot _bot;
    private readonly IPmSendersManager _pmSendersManager;

    protected PrivateMessageParser(IContextFactory contextFactory,
        IBot bot,
        IPmSendersManager pmSendersManager)
    {
        _contextFactory = contextFactory;
        _bot = bot;
        _pmSendersManager = pmSendersManager;
    }

    public sealed override async Task Execute(string[] parts, string roomId = null)
    {
        if (parts.Length > 2 && parts[1] == "pm")
        {
            var context = _contextFactory.GetContext(ContextType.Pm, _bot, parts[4],
                null, _pmSendersManager.GetUser(parts[2]), null);
            await HandlePrivateMessage(context);
        }
    }

    protected abstract Task HandlePrivateMessage(IContext context);
}