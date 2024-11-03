using System.Globalization;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Contexts;

public class RoomContext : Context
{
    private static readonly List<char> RANKS = [' ', '+', '%', '@', '*', '#', '~'];

    private readonly IRoom _room;
    private readonly long _timestamp;

    public RoomContext(IContextProvider contextProvider,
        IBot bot,
        string message,
        string target,
        IUser sender,
        string command,
        IRoom room,
        long timestamp) : base(contextProvider, bot, message, target, sender, command)
    {
        _room = room;
        _timestamp = timestamp;
    }

    public override string RoomId => _room.RoomId;

    public override bool IsPrivateMessage => false;

    public override CultureInfo Culture
    {
        get => _room.Culture;
        set => _room.Culture = value;
    }

    public override ContextType Type => ContextType.Room;

    public override bool HasSufficientRank(char requiredRank)
    {
        return IsSenderWhitelisted || RANKS.IndexOf(Sender.Rank) >= RANKS.IndexOf(requiredRank);
    }

    public override void Reply(string message, bool rankAware = false)
    {
        if (rankAware && !HasSufficientRank('+'))
        {
            Bot.Say(RoomId, $"/pm {Sender.UserId}, {message}");
            return;
        }
        Bot.Say(RoomId, message);
    }

    public override void SendHtml(string html, string roomId = null, bool rankAware = false)
    {
        if (rankAware && !HasSufficientRank('+'))
        {
            Bot.Say(RoomId, $"/pminfobox {Sender.UserId}, {html}");
            return;
        }
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