using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.VoltorbFlip;

[NamedCommand("vfflip")]
public class FlipVoltorbFlipCommand : Command
{
    private readonly IRoomsManager _roomsManager;

    public FlipVoltorbFlipCommand(IRoomsManager roomsManager)
    {
        _roomsManager = roomsManager;
    }

    public override bool IsPrivateMessageOnly => true;
    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var parts = context.Target.Split(',');
        if (parts.Length < 3)
        {
            return;
        }

        var roomId = parts[0].Trim();
        if (!int.TryParse(parts[1].Trim(), out var row))
        {
            return;
        }

        if (!int.TryParse(parts[2].Trim(), out var col))
        {
            return;
        }

        var room = _roomsManager.GetRoom(roomId);
        if (room?.Game is IVoltorbFlipGame voltorbFlip)
        {
            await voltorbFlip.FlipTile(context.Sender, row, col);
        }
    }
}
