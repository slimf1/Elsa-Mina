using System.Globalization;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Rooms.Parameters;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Contexts;

public class RoomContext : Context
{
    private readonly IContextProvider _contextProvider;
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
        _contextProvider = contextProvider;

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

    protected override bool IsAllowingErrorMessages => _contextProvider
        .GetRoomParameterValue(RoomId, RoomParametersConstants.IS_SHOWING_ERROR_MESSAGES).ToBoolean();

    public override bool HasSufficientRank(Rank requiredRank)
    {
        return IsSenderWhitelisted || (int)Sender.Rank >= (int)requiredRank;
    }

    public override void Reply(string message, bool rankAware = false)
    {
        if (rankAware && !HasSufficientRank(Rank.Voiced))
        {
            Bot.Say(RoomId, $"/pm {Sender.UserId}, {message}");
            return;
        }

        Bot.Say(RoomId, message);
    }

    public override void ReplyHtml(string html, string roomId = null, bool rankAware = false)
    {
        if (rankAware && !HasSufficientRank(Rank.Voiced))
        {
            Bot.Say(RoomId, $"/pminfobox {Sender.UserId}, {html}");
            return;
        }

        Bot.Say(RoomId, $"/addhtmlbox {html}");
    }

    public override void SendHtmlTo(string userId, string html, string roomId = null)
    {
        Bot.Say(RoomId, $"/sendprivatehtmlbox {userId}, {html}");
    }

    public override void ReplyUpdatableHtml(string htmlId, string html, bool isChanging)
    {
        var command = isChanging ? "changeuhtml" : "adduhtml";
        Bot.Say(RoomId, $"/{command} {htmlId}, {html}");
    }
}