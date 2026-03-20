using ElsaMina.Commands.Tournaments.Rpplf;

namespace ElsaMina.UnitTests.Commands.Tournaments.Rpplf;

public class TestRpplfTournamentCommand : RpplfTournamentCommand
{
    public override string HelpMessageKey => string.Empty;
    protected override string Format { get; }
    protected override string TourName { get; }
    protected override string TeamsName { get; }
    protected override string TourRules { get; }
    protected override int? AutoDq { get; }

    public TestRpplfTournamentCommand(
        string format = "gen9ubers",
        string tourName = "Test RPPLF",
        string teamsName = "testteams",
        string tourRules = "Item Clause=1",
        int? autoDq = null)
    {
        Format = format;
        TourName = tourName;
        TeamsName = teamsName;
        TourRules = tourRules;
        AutoDq = autoDq;
    }
}
