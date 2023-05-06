using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Contexts;

public class RoomContext : Context
{
    private readonly IList<char> RANKS = new List<char>
    {
        ' ', '+', '%', '@', '*', '#', '&'
    };

    private readonly IConfigurationManager _configurationManager;

    private IRoom _room;
    private long _timestamp;
    private CultureInfo _cultureInfo;

    public RoomContext(IConfigurationManager configurationManager,
        IBot bot,
        string target,
        IUser sender,
        string command,
        IRoom room,
        long timestamp) : base(configurationManager, bot, target, sender, command)
    {
        _configurationManager = configurationManager;

        _room = room;
        _timestamp = timestamp;
    }

    public override string RoomId => _room.RoomId;
    public override ContextType Type => ContextType.Room;
    public override bool IsPm => false;

    public override CultureInfo Locale
    {
        get => new(_room.Locale);
        set
        {
            // TODO
        }
    }

    public override bool HasSufficientRank(char requiredRank)
    {
        return _configurationManager.Configuration?.Whitelist?.Contains(Sender.UserId) == true
               || RANKS.IndexOf(Sender.Rank) >= RANKS.IndexOf(requiredRank);
    }

    public override void Reply(string message)
    {
        Bot.Say(RoomId, message);
    }

    public override void SendHtml(string html, string roomId = null)
    {
        Bot.Say(RoomId, $"/addhtmlbox {html}");
    }

    public override void SendUpdatableHtml(string htmlId, string html, bool isChanging)
    {
        var command = isChanging ? "changeuhtml" : "adduhtml";
        Bot.Say(RoomId, $"/{command} {htmlId}, {html}");
    }
}