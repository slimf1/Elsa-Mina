using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Arcade.Levels;

public class ArcadeLevelsViewModel : LocalizableViewModel
{
    public IReadOnlyDictionary<int, List<ArcadePlayer>> Levels { get; init; }
}