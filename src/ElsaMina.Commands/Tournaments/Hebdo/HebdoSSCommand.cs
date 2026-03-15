using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoss")]
public class HebdoSsCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_ss_help";
    protected override string Format => "gen8ou";
    protected override int Autostart => 8;
    protected override string TourName => "Tournoi Hebdo";
    protected override string WallMessage => "Tournoi Hebdo en SS OU, un Room Prize Winner (^) à la clé !!";
    protected override string RoomEventsName => "Tournoi Hebdo";
}
