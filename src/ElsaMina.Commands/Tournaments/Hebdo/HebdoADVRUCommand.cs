using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoadvru")]
public class HebdoAdvruCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_advru_help";
    protected override string Format => "gen3ru";
    protected override int Autostart => 6;
    protected override string TourName => "RU Classic: ADV";
    protected override string RoomEventsName => "RU Classic";
}
