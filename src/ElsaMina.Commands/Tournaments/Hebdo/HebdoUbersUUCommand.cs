using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoubersuu")]
public class HebdoUbersUuCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_ubersuu_help";
    protected override string Format => "ubersuu";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo UM: Ubers UU";
    protected override string WallMessage => "Hebdo UM en **Ubers UU** !";
}
