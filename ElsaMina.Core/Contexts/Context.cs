using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public abstract class Context : IContext
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    
    public IBot Bot { get; }
    public string Target { get; }
    public IUser Sender { get; }
    public string Command { get; }

    protected Context(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IBot bot,
        string target,
        IUser sender,
        string command)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        
        Bot = bot;
        Target = target;
        Sender = sender;
        Command = command;
    }

    public bool IsSenderWhitelisted => _configurationManager
        .Configuration
        .Whitelist
        .Contains(Sender.UserId);
    
    public void SendHtmlPage(string pageName, string html)
    {
        Bot.Say(RoomId, $"/sendhtmlpage {Sender.UserId}, {pageName}, {html}");
    }
    
    public string GetString(string key)
    {
        var localizedString = _resourcesService.GetString(key, Locale);
        return string.IsNullOrEmpty(localizedString)
            ? key
            : localizedString;
    }

    public string GetString(string key, params object[] formatArguments)
    {
        return string.Format(GetString(key), formatArguments);
    }

    public void ReplyLocalizedMessage(string key, params object[] formatArguments)
    {
        Reply(GetString(key, formatArguments));
    }

    public override string ToString()
    {
        return $"{nameof(Context)}[{nameof(Bot)}: {Bot}, " +
               $"{nameof(Target)}: {Target}, " +
               $"{nameof(Sender)}: {Sender}, " +
               $"{nameof(Command)}: {Command}, " +
               $"{nameof(IsSenderWhitelisted)}: {IsSenderWhitelisted}, " +
               $"{nameof(RoomId)}: {RoomId}, " +
               $"{nameof(Type)}: {Type}, " +
               $"{nameof(IsPm)}: {IsPm}, " +
               $"{nameof(Locale)}: {Locale}]";
    }

    public abstract string RoomId { get; }
    public abstract ContextType Type { get; }
    public abstract bool IsPm { get; }
    public abstract CultureInfo Locale { get; set; }
    
    public abstract bool HasSufficientRank(char requiredRank);
    public abstract void Reply(string message);
    public abstract void SendHtml(string html, string roomId = null);
    public abstract void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}