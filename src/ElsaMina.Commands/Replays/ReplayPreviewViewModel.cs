using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Replays;

public class ReplayPreviewViewModel : LocalizableViewModel
{
    public string Player1 { get; set; }
    public IList<string> Player1Species { get; set; }
    public string Player2 { get; set; }
    public IList<string> Player2Species { get; set; }
    public bool Has4Players { get; set; }
    public string Player3 { get; set; }
    public IList<string> Player3Species { get; set; }
    public string Player4 { get; set; }
    public IList<string> Player4Species { get; set; }
    public int Rating { get; set; }
    public string Format { get; set; } 
    public DateTime Date { get; set; }
    public int Views { get; set; }
}