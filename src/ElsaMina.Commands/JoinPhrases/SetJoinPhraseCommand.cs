using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.Core.Utils;
using ElsaMina.Logging;

namespace ElsaMina.Commands.JoinPhrases;

[NamedCommand("setjoinphrase")]
public class SetJoinPhraseCommand : Command
{
    private readonly IRoomUserDataService _roomUserDataService;

    public SetJoinPhraseCommand(IRoomUserDataService roomUserDataService)
    {
        _roomUserDataService = roomUserDataService;
    }

    public override Rank RequiredRank => Rank.Driver;
    public override string HelpMessageKey => "setjoinphrase_help_message";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
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
            await _roomUserDataService.SetUserJoinPhraseAsync(context.RoomId, userId, joinPhrase, cancellationToken);
            context.ReplyLocalizedMessage("setjoinphrase_success");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while updating join phrase");
            context.ReplyLocalizedMessage("setjoinphrase_failure", exception.Message);
        }
    }
}