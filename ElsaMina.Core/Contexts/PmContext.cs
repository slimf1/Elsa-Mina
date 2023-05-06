using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Contexts;

public class PmContext : Context
{
    private readonly IConfigurationService _configurationService;

    private CultureInfo _currentLocale;

    public PmContext(IConfigurationService configurationService,
        IBot bot,
        string target,
        IUser sender,
        string command) : base(configurationService, bot, target, sender, command)
    {
        _configurationService = configurationService;

        _currentLocale = new CultureInfo(_configurationService.Configuration.DefaultLocaleCode);
    }

    public override string RoomId => _configurationService.Configuration.DefaultRoom;
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

    public override void SendHtml(string html, string? roomId = null)
    {
        Bot.Say(roomId ?? RoomId, $"/pminfobox {Sender.UserId}, {html}");
    }

    public override void SendUpdatableHtml(string htmlId, string html, bool isChanging)
    {
        var command = isChanging ? "pmchangeuhtml" : "pmuhtml";
        Bot.Say(RoomId, $"/{command} {Sender.UserId}, {htmlId}, {html}");
    }
}