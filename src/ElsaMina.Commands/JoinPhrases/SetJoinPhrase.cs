using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.JoinPhrases;

[NamedCommand("setjoinphrase")]
public class SetJoinPhrase : Command
{
    private readonly IRoomUserDataService _roomUserDataService;

    public SetJoinPhrase(IRoomUserDataService roomUserDataService)
    {
        _roomUserDataService = roomUserDataService;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "setjoinphrase_help_message";

    public override async Task Run(IContext context)
    {
        var parts = context.Target.Split(',');
        if (parts.Length != 2)
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        var userId = parts[0].ToLowerAlphaNum();
        var joinPhrase = parts[1].Trim();
        try
        {
            await _roomUserDataService.SetUserJoinPhrase(context.RoomId, userId, joinPhrase);
            context.ReplyLocalizedMessage("setjoinphrase_success");
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Error while updating join phrase");
            context.ReplyLocalizedMessage("setjoinphrase_failure", exception.Message);
        }
    }
}