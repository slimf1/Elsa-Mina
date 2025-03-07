namespace ElsaMina.Commands.Tournaments;

public class TournamentResults
{
    public Dictionary<string, int> WinsCount { get; set; } = [];
    public string Winner { get; set; }
    public string RunnerUp { get; set; }
    public List<string> SemiFinalists { get; set; } = [];
    public List<string> Players { get; set; } = [];
    public string Format { get; set; }
}