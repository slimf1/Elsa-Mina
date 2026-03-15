using ElsaMina.Commands.Tournaments.Hebdo;
using ElsaMina.Core.Services.Commands;

namespace ElsaMina.UnitTests.Commands.Tournaments.Hebdo;

public class TestHebdoTournamentCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => string.Empty;
    protected override string Format { get; }
    protected override int Autostart { get; }
    protected override string TourName { get; }
    protected override string WallMessage { get; }
    protected override string RoomEventsName { get; }

    public TestHebdoTournamentCommand(
        string format = "ou",
        int autostart = 6,
        string tourName = "Test Tour",
        string wallMessage = null,
        string roomEventsName = null)
    {
        Format = format;
        Autostart = autostart;
        TourName = tourName;
        WallMessage = wallMessage;
        RoomEventsName = roomEventsName;
    }
}
