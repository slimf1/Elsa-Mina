using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdocamo")]
public class HebdoCamoCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_camo_help";
    protected override string Format => "camomons";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR Camo";
    protected override string WallMessage => "Hebdo OM FR en Camomons !";
}
