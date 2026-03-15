using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdonfe")]
public class HebdoNfeCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_nfe_help";
    protected override string Format => "nfe";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo UM: NFE";
    protected override string WallMessage => "Hebdo UM en **NFE** !";
}
