using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdobh")]
public class HebdoBhCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_bh_help";
    protected override string Format => "balancedhackmons";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR BH";
    protected override string WallMessage => "Hebdo OM FR en Balanced Hackmons !";
}
