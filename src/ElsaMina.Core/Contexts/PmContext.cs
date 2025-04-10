﻿using System.Globalization;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public class PmContext : Context
{
    public PmContext(IContextProvider contextProvider,
        IBot bot,
        string message,
        string target,
        IUser sender,
        string command) : base(contextProvider, bot, message, target, sender, command)
    {
        Culture = contextProvider.DefaultCulture;
        RoomId = contextProvider.DefaultRoom;
    }

    public override string RoomId { get; }

    public override bool IsPrivateMessage => true;

    public sealed override CultureInfo Culture { get; set; }

    public override ContextType Type => ContextType.Pm;

    public override bool HasSufficientRank(Rank requiredRank) => true;

    protected override bool IsAllowingErrorMessages => true;

    public override void Reply(string message, bool rankAware = false)
        => Bot.Send($"|/pm {Sender.UserId}, {message}");

    public override void ReplyHtml(string html, string roomId = null, bool rankAware = false)
        => Bot.Say(roomId ?? RoomId, $"/pminfobox {Sender.UserId}, {html}");

    public override void SendHtmlTo(string userId, string html, string roomId = null) =>
        Bot.Say(roomId ?? RoomId, $"/pminfobox {userId}, {html}");

    public override void ReplyUpdatableHtml(string htmlId, string html, bool isChanging)
    {
        var command = isChanging ? "pmchangeuhtml" : "pmuhtml";
        Bot.Say(RoomId, $"/{command} {Sender.UserId}, {htmlId}, {html}");
    }
}