using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Commands.Profile;

public class SetAvatar : Command<SetAvatar>, INamed
{
    public static string Name => "avatar";
    public static IEnumerable<string> Aliases => new[] { "set-avatar", "setavatar" };

    private readonly IRoomUserDataService _roomUserDataService;
    private readonly ILogger _logger;

    public SetAvatar(IRoomUserDataService roomUserDataService,
        ILogger logger)
    {
        _roomUserDataService = roomUserDataService;
        _logger = logger;
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
            _logger.Error(exception, "Error while updating avatar");
            context.ReplyLocalizedMessage("avatar_failure", exception.Message);
        }
    }
}