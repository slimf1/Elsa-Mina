using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Arcade;

public class ArcadeLevelsViewModel : LocalizableViewModel
{
    public IDictionary<int, List<string>> Levels { get; set; }
}