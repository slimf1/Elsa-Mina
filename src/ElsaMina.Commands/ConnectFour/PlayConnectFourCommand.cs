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
        var parts = context.Target.Split(',');
        var roomId = parts[0].Trim();
        var play = parts[1].Trim();
        var room = _roomsManager.GetRoom(roomId);
        if (room?.Game is ConnectFourGame connectFour)
        {
            await connectFour.Play(context.Sender, play);
        }
    }
}