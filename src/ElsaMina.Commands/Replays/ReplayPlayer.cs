namespace ElsaMina.Commands.Replays;

public class ReplayPlayer
{
    public string Name { get; set; }
    public IEnumerable<string> Team { get; set; } = [];
}
