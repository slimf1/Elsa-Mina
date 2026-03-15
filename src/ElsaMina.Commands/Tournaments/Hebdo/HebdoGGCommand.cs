using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdogg")]
public class HebdoGgCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_gg_help";
    protected override string Format => "godlygift";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR GG";
    protected override string WallMessage => "Hebdo OM FR en Godly Gift !";
}
