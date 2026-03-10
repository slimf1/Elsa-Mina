using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.VoltorbFlip;

[NamedCommand("vftogglemark")]
public class ToggleMarkVoltorbFlipCommand : Command
{
    private readonly IRoomsManager _roomsManager;

    public ToggleMarkVoltorbFlipCommand(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override bool IsPrivateMessageOnly => true;
    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var roomId = context.Target.Trim();
        var room = _roomsManager.GetRoom(roomId);
        if (room?.Game is IVoltorbFlipGame voltorbFlip)
        {
            await voltorbFlip.ToggleMarkingMode(context.Sender);
        }
    }
}
