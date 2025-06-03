using System.Text;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.Polls;

[NamedCommand("showpolls")]
public class ShowPollsCommand : Command
{
    private readonly ISavedPollRepository _savedPollRepository;
    private readonly IRoomsManager _roomsManager;

    public ShowPollsCommand(ISavedPollRepository savedPollRepository, IRoomsManager roomsManager)
    {
        _savedPollRepository = savedPollRepository;
        _roomsManager = roomsManager;
    }

    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string roomId;
        if (!string.IsNullOrEmpty(context.Target))
        {
            roomId = context.Target.ToLower();
            if (!_roomsManager.HasRoom(roomId))
            {
                context.ReplyLocalizedMessage("show_polls_room_not_exist", roomId);
                return;
            }
        }
        else
        {
            roomId = context.RoomId;
        }

        var pollHistory = await _savedPollRepository.GetPollsByRoomIdAsync(roomId, cancellationToken);
        var savedPolls = pollHistory?.ToArray();
        if (savedPolls == null || savedPolls.Length == 0)
        {
            context.ReplyLocalizedMessage("show_polls_no_polls", roomId);
            return;
        }

        var sb = new StringBuilder();
        sb.Append(context.GetString("show_polls_history_header", roomId));
        foreach (var poll in savedPolls)
        {
            sb.AppendLine(context.GetString("show_polls_history_entry",
                poll.Id,
                poll.EndedAt.ToLocalTime().ToString("G", context.Culture),
                poll.Content));
        }

        context.ReplyLocalizedMessage("show_polls_history_sent");
        context.ReplyHtmlPage("polls-history", sb.ToString());
    }
}