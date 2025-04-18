﻿using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Profile;

[NamedCommand("title", Aliases = ["settitle", "set-title", "set-bio", "setbio"])]
public class SetTitle : Command
{
    private readonly IRoomUserDataService _roomUserDataService;

    public SetTitle(IRoomUserDataService roomUserDataService)
    {
        _roomUserDataService = roomUserDataService;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override string HelpMessageKey => "title_help_message";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var parts = context.Target.Split(",");
        if (parts.Length != 2)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var userId = parts[0].ToLowerAlphaNum();
        var title = parts[1].Trim();
        try
        {
            await _roomUserDataService.SetUserTitle(context.RoomId, userId, title);
            context.ReplyLocalizedMessage("title_success");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while updating avatar");
            context.ReplyLocalizedMessage("title_failure", exception.Message);
        }
    }
}