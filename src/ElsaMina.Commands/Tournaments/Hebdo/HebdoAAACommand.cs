using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoaaa")]
public class HebdoAaaCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_aaa_help";
    protected override string Format => "aaa";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR AAA";
    protected override string WallMessage => "Hebdo OM FR en Almost Any Ability !";
}
