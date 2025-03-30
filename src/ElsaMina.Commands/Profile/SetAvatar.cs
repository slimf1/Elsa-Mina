using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Profile;

[NamedCommand("avatar", Aliases = ["set-avatar", "setavatar"])]
public class SetAvatar : Command
{
    private readonly IRoomUserDataService _roomUserDataService;

    public SetAvatar(IRoomUserDataService roomUserDataService)
    {
        _roomUserDataService = roomUserDataService;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "avatar_help_message";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var parts = context.Target.Split(",");
        if (parts.Length != 2)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var userId = parts[0].ToLowerAlphaNum();
        var avatarUrl = parts[1].Trim();
        try
        {
            await _roomUserDataService.SetUserAvatar(context.RoomId, userId, avatarUrl);
            context.ReplyLocalizedMessage("avatar_success");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while updating avatar");
            context.ReplyLocalizedMessage("avatar_failure", exception.Message);
        }
    }
}