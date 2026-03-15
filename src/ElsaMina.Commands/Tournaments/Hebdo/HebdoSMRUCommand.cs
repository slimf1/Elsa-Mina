using ElsaMina.Core.Services.Commands;

namespace ElsaMina.Commands.Tournaments.Hebdo;

[NamedCommand("hebdosmru")]
public class HebdoSmruCommand : HebdoTournamentCommand
{
    public override string HelpMessageKey => "hebdo_smru_help";
    protected override string Format => "gen7ru";
    protected override int Autostart => 6;
    protected override string TourName => "RU Classic: SM";
    protected override string RoomEventsName => "RU Classic";
}
