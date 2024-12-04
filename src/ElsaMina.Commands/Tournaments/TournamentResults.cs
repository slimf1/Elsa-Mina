namespace ElsaMina.Commands.Tournaments;

public class TournamentResults
{
    public Dictionary<string, int> General { get; set; } = [];

    public string Winner { get; set; }

    public string Finalist { get; set; }

    public List<string> SemiFinalists { get; set; }
    
    public List<string> Players { get; set; }
}