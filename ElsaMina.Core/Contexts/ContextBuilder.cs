using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public class ContextBuilder
{
    private ContextType _contextType;
    private IBot _bot;
    private string _target;
    private IUser _user;
    private string _command;
    private IRoom _room;
    private long _timestamp;

    public ContextBuilder WithType(ContextType contextType)
    {
        _contextType = contextType;
        return this;
    }
    
    public ContextBuilder WithBot(IBot bot)
    {
        _bot = bot;
        return this;
    }

    public ContextBuilder WithTarget(string target)
    {
        _target = target;
        return this;
    }

    public ContextBuilder WithSender(IUser user)
    {
        _user = user;
        return this;
    }

    public ContextBuilder WithCommand(string command)
    {
        _command = command;
        return this;
    }

    public ContextBuilder WithRoom(IRoom room)
    {
        _room = room;
        return this;
    }

    public ContextBuilder WithTimestamp(long timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public IContext Build()
    {
        var configurationManager = DependencyContainerService.s_ContainerService.Resolve<IConfigurationManager>();
        var resourcesService = DependencyContainerService.s_ContainerService.Resolve<IResourcesService>();
        return _contextType switch
        {
            ContextType.Pm => new PmContext(configurationManager, resourcesService, _bot, _target, _user, _command),
            ContextType.Room => new RoomContext(configurationManager, resourcesService, _bot, _target, _user,
                _command, _room, _timestamp),
            _ => throw new ArgumentOutOfRangeException()
        }; 
    }
}