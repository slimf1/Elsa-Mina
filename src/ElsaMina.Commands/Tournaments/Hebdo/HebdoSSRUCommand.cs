using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdossru")]
public class HebdoSsruCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_ssru_help";
    protected override string Format => "gen8ru";
    protected override int Autostart => 6;
    protected override string TourName => "RU Classic: SS";
    protected override string RoomEventsName => "RU Classic";
}
