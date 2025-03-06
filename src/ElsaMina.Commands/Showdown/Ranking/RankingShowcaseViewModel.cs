using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Showdown.Ranking;

public class RankingShowcaseViewModel : LocalizableViewModel
{
    public IEnumerable<RankingDataDto> Rankings { get; set; }
    public string Username { get; set; }
}