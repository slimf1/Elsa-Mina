using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoag")]
public class HebdoAgCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_ag_help";
    protected override string Format => "ag";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo UM: AG";
    protected override string WallMessage => "Hebdo UM en **Anything Goes** !";
}
