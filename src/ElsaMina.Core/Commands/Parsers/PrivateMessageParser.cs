using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Commands.Parsers;

public abstract class PrivateMessageParser : Parser
{
    private readonly IContextFactory _contextFactory;
    private readonly IBot _bot;
    private readonly IPmSendersManager _pmSendersManager;
    private readonly IConfigurationManager _configurationManager;

    protected PrivateMessageParser(IDependencyContainerService dependencyContainerService)
    {
        _contextFactory = dependencyContainerService.Resolve<IContextFactory>();
        _bot = dependencyContainerService.Resolve<IBot>();
        _pmSendersManager = dependencyContainerService.Resolve<IPmSendersManager>();
        _configurationManager = dependencyContainerService.Resolve<IConfigurationManager>();
    }

    protected sealed override async Task Execute(string[] parts, string roomId = null)
    {
        if (parts.Length > 2 && parts[1] == "pm")
        {
            string target = null;
            string command = null;
            if (parts[4].StartsWith(_configurationManager.Configuration.Trigger))
            {
                (target, command) = Parsing.ParseMessage(parts[4], _configurationManager.Configuration.Trigger);
            }
            var context = _contextFactory.GetContext(ContextType.Pm, _bot, parts[4],
                target, _pmSendersManager.GetUser(parts[2]), command);
            await HandlePrivateMessage(context);
        }
    }

    protected abstract Task HandlePrivateMessage(IContext context);
}