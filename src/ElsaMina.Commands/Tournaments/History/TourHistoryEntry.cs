namespace ElsaMina.Commands.Tournaments.History;

public record TourHistoryEntry(
    int Id,
    string Format,
    string Winner,
    string RunnerUp,
    IReadOnlyList<string> SemiFinalists,
    int PlayerCount,
    string EndedAt);
