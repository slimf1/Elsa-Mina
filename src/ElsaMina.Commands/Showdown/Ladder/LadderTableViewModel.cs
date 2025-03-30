using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Showdown.Ladder;

public class LadderTableViewModel : LocalizableViewModel
{
    public string Format { get; init; }
    public IEnumerable<LadderPlayerDto> TopList { get; init; }
    public bool ShowInnerRanking { get; init; }
}