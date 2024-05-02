using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameResultViewModel : LocalizableViewModel
{
    public IDictionary<string, int> Scores { get; set; }
}