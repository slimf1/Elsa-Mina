using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameResultViewModel : LocalizableViewModel
{
    public IReadOnlyDictionary<string, int> Scores { get; init; }
}