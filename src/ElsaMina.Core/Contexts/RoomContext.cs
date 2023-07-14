using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public class RoomContext : Context
{
    private readonly IList<char> RANKS = new List<char>
    {
        ' ', '+', '%', '@', '*', '#', '&'
    };

    private readonly IRoom _room;
    private readonly long _timestamp;

    public RoomContext(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IBot bot,
        string message,
        string target,
        IUser sender,
        string command,
        IRoom room,
        long timestamp) : base(configurationManager, resourcesService, bot, message, target, sender, command)
    {
        _room = room;
        _timestamp = timestamp;
    }

    public override string RoomId => _room.RoomId;
    public override bool IsPm => false;

    public override CultureInfo Locale
    {
        get => new(_room.Locale);
        set => _room.Locale = value.Name;
    }

    public override bool HasSufficientRank(char requiredRank)
    {
        return IsSenderWhitelisted || RANKS.IndexOf(Sender.Rank) >= RANKS.IndexOf(requiredRank);
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

    public override string ToString()
    {
        return $"{nameof(RoomContext)}[{base.ToString()}," +
               $"{nameof(_room)}: {_room}, " +
               $"{nameof(_timestamp)}: {_timestamp}]";
    }
}