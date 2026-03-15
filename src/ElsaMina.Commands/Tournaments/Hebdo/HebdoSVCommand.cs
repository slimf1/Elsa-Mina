using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdosv")]
public class HebdoSvCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_sv_help";
    protected override string Format => "ou";
    protected override int Autostart => 8;
    protected override string TourName => "Tournoi Hebdo";
    protected override string WallMessage => "Tournoi Hebdo en SV OU, un Room Prize Winner (^) à la clé !!";
    protected override string RoomEventsName => "Tournoi Hebdo";
}
