using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Arcade.Sheets;

public class ArcadeHallOfFameViewModel : LocalizableViewModel
{
    public ArcadeHallOfFameEntry[] Entries { get; set; } = [];
    public string SpreadsheetUrl { get; set; } = string.Empty;
}