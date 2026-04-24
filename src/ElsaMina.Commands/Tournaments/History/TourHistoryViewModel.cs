using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Tournaments.History;

public class TourHistoryViewModel : LocalizableViewModel
{
    public string Room { get; init; }
    public IReadOnlyList<TourHistoryEntry> Entries { get; init; }
}
