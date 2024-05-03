using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Profile;

public class SetAvatar : Command<SetAvatar>, INamed
{
    public static string Name => "avatar";
    public static List<string> Aliases => ["set-avatar", "setavatar"];

    private readonly IRoomUserDataService _roomUserDataService;

    public SetAvatar(IRoomUserDataService roomUserDataService)
    {
        _roomUserDataService = roomUserDataService;
    }
    
    public override char RequiredRank => '%';
    public override string HelpMessageKey => "avatar_help_message";

    public override async Task Run(IContext context)
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
            Logger.Current.Error(exception, "Error while updating avatar");
            context.ReplyLocalizedMessage("avatar_failure", exception.Message);
        }
    }
}