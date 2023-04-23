using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Contexts;

public abstract class Context
{
    private readonly IConfigurationService _configurationService;
    
    public IBot Bot { get; }
    public string Target { get; }
    public IUser Sender { get; }
    public string Command { get; }

    private string DefaultRoom => _configurationService.Configuration?.DefaultRoom ?? string.Empty;

    protected Context(IConfigurationService configurationService,
        IBot bot,
        string target,
        IUser sender,
        string command)
    {
        _configurationService = configurationService;
        
        Bot = bot;
        Target = target;
        Sender = sender;
        Command = command;
    }

    public bool IsSenderWhitelisted => _configurationService
        .Configuration?
        .Whitelist?
        .Contains(Sender.UserId) == true;
    
    public void SendHtmlPage(string pageName, string html)
    {
        Bot.Say(DefaultRoom, $"/sendhtmlpage {Sender.UserId}, {pageName}, {html}");
    }

    public abstract string RoomId { get; }
    public abstract ContextType Type { get; }
    public abstract bool IsPm { get; }
    public abstract CultureInfo Locale { get; set; }
    
    public abstract bool HasSufficientRank(char requiredRank);
    public abstract void Reply(string message);
    public abstract void SendHtml(string html, string? roomId = null);
    public abstract void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}