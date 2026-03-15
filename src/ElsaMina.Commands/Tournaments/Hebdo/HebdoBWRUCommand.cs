using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdobwru")]
public class HebdoBwruCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_bwru_help";
    protected override string Format => "gen5ru";
    protected override int Autostart => 6;
    protected override string TourName => "RU Classic: BW";
    protected override string RoomEventsName => "RU Classic";
}
