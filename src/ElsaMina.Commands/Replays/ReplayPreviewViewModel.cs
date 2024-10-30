using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Replays;

public class ReplayPreviewViewModel : LocalizableViewModel
{
    public IList<ReplayPlayer> Players { get; set; } = [];
    public int Rating { get; set; }
    public string Format { get; set; } 
    public DateTime Date { get; set; }
    public int Views { get; set; }
}