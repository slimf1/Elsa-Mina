using ElsaMina.Core.Services.Dex;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.GuessingGame.Gatekeepers;

public class GatekeepersGamePanelViewModel : LocalizableViewModel
{
    public string FootprintSprite { get; set; }
    IEnumerable<Pokemon> CurrentOptions { get; set; }
    public bool ShowSilhouettes { get; set; }
    public bool ShowPortraits { get; set; }
    public IReadOnlyDictionary<string, int> Scores { get; set; }
    public int CurrentTurn { get; set; }
}