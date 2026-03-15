using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdosm")]
public class HebdoSmCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_sm_help";
    protected override string Format => "gen7ou";
    protected override int Autostart => 8;
    protected override string TourName => "Tournoi Hebdo";
    protected override string WallMessage => "Tournoi Hebdo en SM OU, un Room Prize Winner (^) à la clé !!";
    protected override string RoomEventsName => "Tournoi Hebdo";
}
