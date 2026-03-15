using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdozu")]
public class HebdoZuCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_zu_help";
    protected override string Format => "zu";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo UM: ZU";
    protected override string WallMessage => "Hebdo UM en **ZU** !";
}
