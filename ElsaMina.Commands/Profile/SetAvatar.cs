using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using Serilog;

namespace ElsaMina.Commands.Profile;

public class SetAvatar : ICommand
{
    public static string Name => "avatar";
    public static IEnumerable<string> Aliases => new[] { "set-avatar", "setavatar" };
    public char RequiredRank => '%';
    public string HelpMessageKey => "avatar_help_message";

    private readonly IRoomUserDataService _roomUserDataService;
    private readonly ILogger _logger;

    public SetAvatar(IRoomUserDataService roomUserDataService,
        ILogger logger)
    {
        _roomUserDataService = roomUserDataService;
        _logger = logger;
    }

    public async Task Run(IContext context)
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