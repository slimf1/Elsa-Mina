using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdopic")]
public class HebdoPiCCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_pic_help";
    protected override string Format => "partnersincrime";
    protected override int Autostart => 6;
    protected override string TourName => "Hebdo OM FR PiC";
    protected override string WallMessage => "Hebdo OM FR en Partners in Crime !";
}
