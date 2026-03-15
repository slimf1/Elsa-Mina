using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdomnm")]
public class HebdoMnMCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_mnm_help";
    protected override string Format => "mixandmega";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR MnM";
    protected override string WallMessage => "Hebdo OM FR en Mix and Mega !";
}
