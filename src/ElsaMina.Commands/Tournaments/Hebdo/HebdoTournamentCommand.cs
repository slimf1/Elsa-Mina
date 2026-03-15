using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Tournaments.Hebdo;

public abstract class HebdoTournamentCommand : Command
{
    private const string ARCADE_ROOM_ID = "arcade";

    protected abstract string Format { get; }
    protected abstract int Autostart { get; }
    protected abstract string TourName { get; }
    protected virtual string WallMessage => null;
    protected virtual string RoomEventsName => null;

    public override Rank RequiredRank => Rank.Driver;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (context.RoomId != ARCADE_ROOM_ID)
        {
            context.ReplyLocalizedMessage("hebdo_wrong_room");
            return Task.CompletedTask;
        }

        context.Reply($"/tour create {Format}, elim");
        context.Reply($"/tour autostart {Autostart}");
        context.Reply($"/tour name {TourName}");

        if (WallMessage != null)
        {
            context.Reply($"/wall {WallMessage}");
        }

        if (RoomEventsName != null)
        {
            context.Reply($"/roomevents start {RoomEventsName}");
        }

        return Task.CompletedTask;
    }
}
