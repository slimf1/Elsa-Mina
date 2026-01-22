using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameResultViewModel : LocalizableViewModel
{
    public IReadOnlyDictionary<string, int> Scores { get; init; }
}