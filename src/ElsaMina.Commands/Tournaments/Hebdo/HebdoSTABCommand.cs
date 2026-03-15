using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdostab")]
public class HebdoStabCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_stab_help";
    protected override string Format => "stabmons";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR STAB";
    protected override string WallMessage => "Hebdo OM FR en STABmons !";
}
