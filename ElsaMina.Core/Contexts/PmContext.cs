using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public class PmContext : Context
{
    private readonly IConfigurationManager _configurationManager;

    private CultureInfo _currentLocale;

    public PmContext(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IBot bot,
        string target,
        IUser sender,
        string command) : base(configurationManager, resourcesService, bot, target, sender, command)
    {
        _configurationManager = configurationManager;

        _currentLocale = new CultureInfo(_configurationManager.Configuration.DefaultLocaleCode);
    }

    public override string RoomId => _configurationManager.Configuration.DefaultRoom;
    public override ContextType Type => ContextType.Pm;
    public override bool IsPm => true;

    public override CultureInfo Locale
    {
        get => _currentLocale;
        set => _currentLocale = value;
    }
    
    public override bool HasSufficientRank(char requiredRank)
    {
        return true;
    }

    public override void Reply(string message)
    {
        Bot.Send($"|/pm {Sender.UserId}, {message}");
    }

    public override void SendHtml(string html, string roomId = null)
    {
        Bot.Say(roomId ?? RoomId, $"/pminfobox {Sender.UserId}, {html}");
    }

    public override void SendUpdatableHtml(string htmlId, string html, bool isChanging)
    {
        var command = isChanging ? "pmchangeuhtml" : "pmuhtml";
        Bot.Say(RoomId, $"/{command} {Sender.UserId}, {htmlId}, {html}");
    }
}