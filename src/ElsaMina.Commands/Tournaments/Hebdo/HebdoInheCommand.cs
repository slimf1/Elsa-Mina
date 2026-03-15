using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoinhe")]
public class HebdoInheCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_inhe_help";
    protected override string Format => "inheritance";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR Inheritance";
    protected override string WallMessage => "Hebdo OM FR en Inheritance !";
}
