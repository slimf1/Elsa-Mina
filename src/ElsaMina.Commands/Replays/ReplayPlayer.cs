namespace ElsaMina.Commands.Replays;

public class ReplayPlayer
{
    public string Name { get; init; }
    public IEnumerable<string> Team { get; init; } = [];
}
