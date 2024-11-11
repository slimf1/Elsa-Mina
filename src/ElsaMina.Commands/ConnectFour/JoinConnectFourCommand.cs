using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("c4join", Aliases = ["c4j"])]
public class JoinConnectFourCommand : Command
{
    private readonly IRoomsManager _roomsManager;

    public JoinConnectFourCommand(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsPrivateMessageOnly => true;

    public override async Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.Target);
        if (room?.Game is not ConnectFourGame connectFour)
        {
            return;
        }

        await connectFour.JoinGame(context.Sender);
        if (!connectFour.IsStarted)
        {
            await connectFour.DisplayAnnounce(); // Gets updated
        }
    }
}