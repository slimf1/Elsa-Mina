using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdoorasru")]
public class HebdoOrasruCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_orasru_help";
    protected override string Format => "gen6ru";
    protected override int Autostart => 6;
    protected override string TourName => "RU Classic: ORAS";
    protected override string RoomEventsName => "RU Classic";
}
