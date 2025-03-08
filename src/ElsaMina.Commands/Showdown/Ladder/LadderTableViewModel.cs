using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Showdown.Ladder;

public class LadderTableViewModel : LocalizableViewModel
{
    public string Format { get; set; }
    public IEnumerable<LadderPlayerDto> TopList { get; set; }
    public bool ShowInnerRanking { get; set; }
}