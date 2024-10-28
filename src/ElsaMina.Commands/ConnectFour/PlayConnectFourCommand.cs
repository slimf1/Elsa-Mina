using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

[NamedCommand("c4play")]
public class PlayConnectFourCommand : Command
{
    private readonly IRoomsManager _roomsManager;

    public PlayConnectFourCommand(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override bool IsPrivateMessageOnly => true;
    public override bool IsAllowedInPrivateMessage => true;

    public override async Task Run(IContext context)
    {
        var room = _roomsManager.GetRoom(context.RoomId);
        if (room?.Game is ConnectFour connectFour)
        {
            await connectFour.Play(context.Sender, context.Target);
        }
    }
}