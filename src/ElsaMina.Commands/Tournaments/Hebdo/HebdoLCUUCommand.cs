using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdolcuu")]
public class HebdoLcuuCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_lcuu_help";
    protected override string Format => "lcuu";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo UM: LC UU";
    protected override string WallMessage => "Hebdo UM en **LC UU** !";
}
