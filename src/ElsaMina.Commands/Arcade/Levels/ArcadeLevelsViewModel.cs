using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Arcade.Levels;

public class ArcadeLevelsViewModel : LocalizableViewModel
{
    public IReadOnlyDictionary<int, List<string>> Levels { get; init; }
}