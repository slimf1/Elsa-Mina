using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdo1v1")]
public class Hebdo1V1Command : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_1v1_help";
    protected override string Format => "1v1";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo UM: 1v1";
    protected override string WallMessage => "Hebdo UM en **1v1** !";
}
