using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Badges.HallOfFame;

public class HallOfFameViewModel : LocalizableViewModel
{
    public PlayerRecord[] SortedPlayerRecords { get; set; }
}